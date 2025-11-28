using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.EmployeeContract.Req;
using StoreSystem.Application.Contract.EmployeeContract.Res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<StoreEmployee> _empRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IMemoryCache _cache;

        public EmployeeService(IRepository<StoreEmployee> empRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, IMemoryCache cache)
        {
            _empRepo = empRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
        }

        private string GetAllCacheKey(int p, int s) => $"employees:all:{p}:{s}";
        private string GetByIdCacheKey(int id) => $"employee:{id}";

        public async Task<GeneralResponse<int>> CreateEmployeeAsync(EmployeeReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);
            var entity = _mapper.Map<StoreEmployee>(req);
            await _empRepo.AddAsync(entity);
            await _uow.CompleteAsync();
            _cache.Remove("employees:all:1:10");
            return GeneralResponse<int>.Success(entity.Id, "Employee created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateEmployeeAsync(int id, EmployeeReq req)
        {
            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var entity = await _empRepo.FindAsync(x => x.Id == id);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);
            entity.UserId = req.UserId;
            entity.StoreId = req.StoreId;
            entity.RoleId = req.Role; // Assuming Role in Req maps to RoleId
            // entity.IsActive = req.IsActive; // StoreEmployee doesn't seem to have IsActive based on file view
            var ok = await _empRepo.UpdateAsync(entity);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update", 500);
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdCacheKey(id));
            _cache.Remove("employees:all:1:10");
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteEmployeeAsync(int id)
        {
            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var entity = await _empRepo.FindAsync(x => x.Id == id);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);
            _empRepo.DeleteAsync(entity);
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdCacheKey(id));
            _cache.Remove("employees:all:1:10");
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<EmployeeRes?>> GetByIdAsync(int id)
        {
            if (id < 1) return GeneralResponse<EmployeeRes?>.Failure("Invalid id", 400);
            var key = GetByIdCacheKey(id);
            if (!_cache.TryGetValue(key, out EmployeeRes? cached))
            {
                var entity = await _empRepo.FindAsync(x => x.Id == id);
                if (entity == null) return GeneralResponse<EmployeeRes?>.Failure("Not found", 404);
                cached = _mapper.Map<EmployeeRes>(entity);
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<EmployeeRes?>.Success(cached, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<EmployeeRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var key = GetAllCacheKey(pageNumber, pageSize);
            if (!_cache.TryGetValue(key, out PagedResult<EmployeeRes>? cached))
            {
                var page = await _empRepo.GetAllAsync(pageNumber, pageSize);
                cached = new PagedResult<EmployeeRes> { Items = page.Items.Select(x => _mapper.Map<EmployeeRes>(x)), PageNumber = page.PageNumber, PageSize = page.PageSize, TotalItems = page.TotalItems };
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<PagedResult<EmployeeRes>>.Success(cached!, "Ok", 200);
        }
    }
}
