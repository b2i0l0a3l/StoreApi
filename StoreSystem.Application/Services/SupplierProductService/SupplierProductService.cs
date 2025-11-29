using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.SupplierProductContract.Req;
using StoreSystem.Application.Contract.SupplierProductContract.Res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Services.SupplierProductService
{
    public class SupplierProductService : ISupplierProductService
    {
        private readonly IRepository<SupplierProduct> _repo;
        private readonly IRepository<Supplier> _supplierRepo;
        private readonly IUniteOfWork _uow;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ICurrentUserService _currentUserService;

        public SupplierProductService(IRepository<SupplierProduct> repo, IRepository<Supplier> supplierRepo, IUniteOfWork uow, AutoMapper.IMapper mapper, IMemoryCache cache, ICurrentUserService currentUserService)
        {
            _repo = repo;
            _supplierRepo = supplierRepo;
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
            _currentUserService = currentUserService;
        }

        private string GetSupplierProductsKey(int supplierId, int page, int pageSize) => $"supplier:{supplierId}:products:{page}:{pageSize}:{_currentUserService.StoreId}";
        private string GetByIdKey(int id) => $"supplierprod:{id}:{_currentUserService.StoreId}";

        public async Task<GeneralResponse<int>> CreateAsync(SupplierProductReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            // Verify the supplier belongs to the current store
            var supplier = await _supplierRepo.FindAsync(s => s.Id == req.SupplierId && s.StoreId == _currentUserService.StoreId.Value);
            if (supplier == null) return GeneralResponse<int>.Failure("Supplier not found", 404);

            var entity = _mapper.Map<SupplierProduct>(req);
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _repo.AddAsync(entity);
            await _uow.CompleteAsync();
            // clear caches
            _cache.Remove(GetSupplierProductsKey(req.SupplierId, 1, 20));
            return GeneralResponse<int>.Success(entity.Id, "Created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateAsync(int id, SupplierProductReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);

            // Verify the supplier belongs to the current store
            var supplier = await _supplierRepo.FindAsync(s => s.Id == req.SupplierId && s.StoreId == _currentUserService.StoreId.Value);
            if (supplier == null) return GeneralResponse<bool?>.Failure("Supplier not found", 404);

            var entity = await _repo.FindAsync(x => x.Id == id);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);

            // Verify the existing supplier also belongs to the current store
            var existingSupplier = await _supplierRepo.FindAsync(s => s.Id == entity.SupplierId && s.StoreId == _currentUserService.StoreId.Value);
            if (existingSupplier == null) return GeneralResponse<bool?>.Failure("Unauthorized", 403);

            entity.SupplierId = req.SupplierId;
            entity.ProductId = req.ProductId;
            entity.CostPrice = req.CostPrice;
            entity.SupplierSku = req.SupplierSku;
            entity.UpdateByUserId = _currentUserService.UserId;

            var ok = await _repo.UpdateAsync(entity);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update", 500);
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdKey(id));
            _cache.Remove(GetSupplierProductsKey(req.SupplierId, 1, 20));
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var entity = await _repo.FindAsync(x => x.Id == id);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);

            // Verify the supplier belongs to the current store
            var supplier = await _supplierRepo.FindAsync(s => s.Id == entity.SupplierId && s.StoreId == _currentUserService.StoreId.Value);
            if (supplier == null) return GeneralResponse<bool?>.Failure("Unauthorized", 403);

            _repo.DeleteAsync(entity); 
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdKey(id));
            _cache.Remove(GetSupplierProductsKey(entity.SupplierId, 1, 20));
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<SupplierProductRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<SupplierProductRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<SupplierProductRes?>.Failure("Invalid id", 400);
            var key = GetByIdKey(id);
            if (!_cache.TryGetValue(key, out SupplierProductRes? cached))
            {
                var entity = await _repo.FindAsync(x => x.Id == id);
                if (entity == null) return GeneralResponse<SupplierProductRes?>.Failure("Not found", 404);

                // Verify the supplier belongs to the current store
                var supplier = await _supplierRepo.FindAsync(s => s.Id == entity.SupplierId && s.StoreId == _currentUserService.StoreId.Value);
                if (supplier == null) return GeneralResponse<SupplierProductRes?>.Failure("Unauthorized", 403);

                cached = _mapper.Map<SupplierProductRes>(entity);
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<SupplierProductRes?>.Success(cached, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<SupplierProductRes>>> GetAllBySupplierAsync(int supplierId, int page, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<SupplierProductRes>>.Failure("Unauthorized", 401);

            // Verify the supplier belongs to the current store
            var supplier = await _supplierRepo.FindAsync(s => s.Id == supplierId && s.StoreId == _currentUserService.StoreId.Value);
            if (supplier == null) return GeneralResponse<PagedResult<SupplierProductRes>>.Failure("Supplier not found", 404);

            var key = GetSupplierProductsKey(supplierId, page, pageSize);
            if (!_cache.TryGetValue(key, out PagedResult<SupplierProductRes>? cached))
            {
                var pageRes = await _repo.GetAllAsync(page, pageSize, x => x.SupplierId == supplierId);
                cached = new PagedResult<SupplierProductRes> { Items = pageRes.Items.Select(x => _mapper.Map<SupplierProductRes>(x)), PageNumber = pageRes.PageNumber, PageSize = pageRes.PageSize, TotalItems = pageRes.TotalItems };
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<PagedResult<SupplierProductRes>>.Success(cached!, "Ok", 200);
        }
    }
}
