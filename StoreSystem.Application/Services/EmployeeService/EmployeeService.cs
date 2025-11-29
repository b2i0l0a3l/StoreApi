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
        private readonly ICurrentUserService _currentUserService;

        public EmployeeService(IRepository<StoreEmployee> empRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, IMemoryCache cache, ICurrentUserService currentUserService)
        {
            _empRepo = empRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
            _currentUserService = currentUserService;
        }

        private string GetAllCacheKey(int p, int s) => $"employees:all:{p}:{s}:{_currentUserService.StoreId}";
        private string GetByIdCacheKey(int id) => $"employee:{id}:{_currentUserService.StoreId}";

        public async Task<GeneralResponse<int>> CreateEmployeeAsync(EmployeeReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);
            
            var entity = _mapper.Map<StoreEmployee>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _empRepo.AddAsync(entity);
            await _uow.CompleteAsync();
            _cache.Remove(GetAllCacheKey(1, 10));
            return GeneralResponse<int>.Success(entity.Id, "Employee created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateEmployeeAsync(int id, EmployeeReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var entity = await _empRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);
            
            entity.UserId = req.UserId;
            entity.RoleId = req.Role;
            entity.UpdateByUserId = _currentUserService.UserId;

            var ok = await _empRepo.UpdateAsync(entity);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update", 500);
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdCacheKey(id));
            _cache.Remove(GetAllCacheKey(1, 10));
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteEmployeeAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var entity = await _empRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (entity == null) return GeneralResponse<bool?>.Failure("Not found", 404);
            
            _empRepo.DeleteAsync(entity);
            await _uow.CompleteAsync();
            _cache.Remove(GetByIdCacheKey(id));
            _cache.Remove(GetAllCacheKey(1, 10));
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<EmployeeRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<EmployeeRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<EmployeeRes?>.Failure("Invalid id", 400);
            var key = GetByIdCacheKey(id);
            if (!_cache.TryGetValue(key, out EmployeeRes? cached))
            {
                var entity = await _empRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
                if (entity == null) return GeneralResponse<EmployeeRes?>.Failure("Not found", 404);
                cached = _mapper.Map<EmployeeRes>(entity);
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<EmployeeRes?>.Success(cached, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<EmployeeRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<EmployeeRes>>.Failure("Unauthorized", 401);

            var key = GetAllCacheKey(pageNumber, pageSize);
            if (!_cache.TryGetValue(key, out PagedResult<EmployeeRes>? cached))
            {
                var page = await _empRepo.GetAllAsync(pageNumber, pageSize, x => x.StoreId == _currentUserService.StoreId.Value);
                cached = new PagedResult<EmployeeRes> { Items = page.Items.Select(x => _mapper.Map<EmployeeRes>(x)), PageNumber = page.PageNumber, PageSize = page.PageSize, TotalItems = page.TotalItems };
                _cache.Set(key, cached, TimeSpan.FromMinutes(5));
            }
            return GeneralResponse<PagedResult<EmployeeRes>>.Success(cached!, "Ok", 200);
        }
    }
}
