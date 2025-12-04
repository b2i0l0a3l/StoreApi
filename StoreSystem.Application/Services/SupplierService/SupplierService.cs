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
        private readonly ICurrentUserService _currentUserService;

        public SupplierService(IRepository<Supplier> supplierRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, IMemoryCache cache, ICurrentUserService currentUserService)
        {
            _supplierRepo = supplierRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreateSupplierAsync(SupplierReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = _mapper.Map<Supplier>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _supplierRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.Publish(new StoreSystem.Core.Events.SupplierEvent.SupplierAddedEvent(entity.Id, entity.Name));

            return GeneralResponse<int>.Success(entity.Id, "Supplier created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateSupplierAsync(int id, SupplierReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var sup = await _supplierRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (sup == null) return GeneralResponse<bool?>.Failure("Supplier not found", 404);

            sup.Name = req.Name;
            sup.ContactName = req.ContactName;
            sup.Phone = req.Phone;
            sup.Email = req.Email;
            sup.UpdateByUserId = _currentUserService.UserId;

            var ok = await _supplierRepo.UpdateAsync(sup);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update supplier", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteSupplierAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var sup = await _supplierRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (sup == null) return GeneralResponse<bool?>.Failure("Supplier not found", 404);

            _supplierRepo.DeleteAsync(sup);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<SupplierRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<SupplierRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<SupplierRes?>.Failure("Invalid id", 400);
            var key = $"supplier:{id}";
            if (!_cache.TryGetValue(key, out Supplier? sup))
            {
                sup = await _supplierRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
                if (sup == null) return GeneralResponse<SupplierRes?>.Failure("Not found", 404);
                _cache.Set(key, sup, TimeSpan.FromMinutes(5));
            }
            else
            {
                // Verify cached supplier belongs to current store
                if (sup!.StoreId != _currentUserService.StoreId.Value)
                    return GeneralResponse<SupplierRes?>.Failure("Not found", 404);
            }
            return GeneralResponse<SupplierRes?>.Success(_mapper.Map<SupplierRes>(sup), "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<SupplierRes>>> GetAllAsync(GetSupplierReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<SupplierRes>>.Failure("Unauthorized", 401);

            var page = await _supplierRepo.GetAllAsync(req.PageNumber, req.PageSize, x => 
                x.StoreId == _currentUserService.StoreId.Value &&
                (string.IsNullOrEmpty(req.Name) || x.Name.Contains(req.Name)) &&
                (string.IsNullOrEmpty(req.ContactName) || (x.ContactName != null && x.ContactName.Contains(req.ContactName))) &&
                (string.IsNullOrEmpty(req.Phone) || (x.Phone != null && x.Phone.Contains(req.Phone))));

            var mapped = new PagedResult<SupplierRes>
            {
                Items = page.Items.Select(i => _mapper.Map<SupplierRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            var cacheKey = $"suppliers:all:{req.PageNumber}:{req.PageSize}:{_currentUserService.StoreId.Value}:{req.Name}:{req.ContactName}:{req.Phone}";
            _cache.Set(cacheKey, mapped, TimeSpan.FromMinutes(5));
            return GeneralResponse<PagedResult<SupplierRes>>.Success(mapped, "Ok", 200);
        }
    }
}
