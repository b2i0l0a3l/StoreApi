using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using StoreSystem.Application.Contract.SupplierContract.Req;
using StoreSystem.Application.Contract.SupplierContract.Res;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Services.SupplierService
{
    /// <summary>
    /// Supplier service implementation.
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _supplierRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IMemoryCache _cache;

        public SupplierService(IRepository<Supplier> supplierRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, IMemoryCache cache)
        {
            _supplierRepo = supplierRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<GeneralResponse<int>> CreateSupplierAsync(SupplierReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = _mapper.Map<Supplier>(req);
            await _supplierRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.Publish(new StoreSystem.Core.Events.SupplierEvent.SupplierAddedEvent(entity.Id, entity.Name));

            return GeneralResponse<int>.Success(entity.Id, "Supplier created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateSupplierAsync(int id, SupplierReq req)
        {
            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var sup = await _supplierRepo.FindAsync(x => x.Id == id);
            if (sup == null) return GeneralResponse<bool?>.Failure("Supplier not found", 404);

            sup.Name = req.Name;
            sup.ContactName = req.ContactName;
            sup.Phone = req.Phone;
            sup.Email = req.Email;

            var ok = await _supplierRepo.UpdateAsync(sup);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update supplier", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteSupplierAsync(int id)
        {
            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var sup = await _supplierRepo.FindAsync(x => x.Id == id);
            if (sup == null) return GeneralResponse<bool?>.Failure("Supplier not found", 404);

            _supplierRepo.DeleteAsync(sup);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<SupplierRes?>> GetByIdAsync(int id)
        {
            if (id < 1) return GeneralResponse<SupplierRes?>.Failure("Invalid id", 400);
            var key = $"supplier:{id}";
            if (!_cache.TryGetValue(key, out Supplier? sup))
            {
                sup = await _supplierRepo.FindAsync(x => x.Id == id);
                if (sup == null) return GeneralResponse<SupplierRes?>.Failure("Not found", 404);
                _cache.Set(key, sup, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<SupplierRes?>.Success(_mapper.Map<SupplierRes>(sup), "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<SupplierRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var page = await _supplierRepo.GetAllAsync(pageNumber, pageSize);
            var mapped = new PagedResult<SupplierRes>
            {
                Items = page.Items.Select(i => _mapper.Map<SupplierRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            var cacheKey = $"suppliers:all:{pageNumber}:{pageSize}";
            _cache.Set(cacheKey, mapped, TimeSpan.FromMinutes(5));
            return GeneralResponse<PagedResult<SupplierRes>>.Success(mapped, "Ok", 200);
        }
    }
}
