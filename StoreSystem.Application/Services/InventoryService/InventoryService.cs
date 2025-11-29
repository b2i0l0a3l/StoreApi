using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using System.Threading.Tasks;
using BookingSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.InventoryContract.req;
using StoreSystem.Application.Contract.InventoryContract.res;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Services.InventoryService
{
    /// <summary>
    /// Inventory service implementation.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public InventoryService(IRepository<Inventory> inventoryRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _inventoryRepo = inventoryRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreateInventoryAsync(InventoryReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = _mapper.Map<Inventory>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _inventoryRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            // No domain event here by default
            return GeneralResponse<int>.Success(entity.Id, "Inventory created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateInventoryAsync(int id, InventoryReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<bool?>.Failure("Inventory not found", 404);

            inv.Name = req.Name;
            inv.Location = req.Location;
            inv.UpdateByUserId = _currentUserService.UserId;

            var ok = await _inventoryRepo.UpdateAsync(inv);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update inventory", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteInventoryAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<bool?>.Failure("Inventory not found", 404);

            _inventoryRepo.DeleteAsync(inv);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<InventoryRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<InventoryRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<InventoryRes?>.Failure("Invalid id", 400);
            var inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<InventoryRes?>.Failure("Not found", 404);

            var res = _mapper.Map<InventoryRes>(inv);
            return GeneralResponse<InventoryRes?>.Success(res, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<InventoryRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<InventoryRes>>.Failure("Unauthorized", 401);

            var page = await _inventoryRepo.GetAllAsync(pageNumber, pageSize, x => x.StoreId == _currentUserService.StoreId.Value);
            var mapped = new PagedResult<InventoryRes>
            {
                Items = page.Items.Select(i => _mapper.Map<InventoryRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<InventoryRes>>.Success(mapped, "Ok", 200);
        }
    }
}
