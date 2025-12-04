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

    public class InventoryService : IInventoryService
    {
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IUniteOfWork _uow;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public InventoryService(IRepository<Inventory> inventoryRepo, IUniteOfWork uow, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _inventoryRepo = inventoryRepo;
            _uow = uow;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreateInventoryAsync(InventoryReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            Inventory entity = _mapper.Map<Inventory>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _inventoryRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            return GeneralResponse<int>.Success(entity.Id, "Inventory created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateInventoryAsync(int id, InventoryReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);

            Inventory? inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<bool?>.Failure("Inventory not found", 404);

            inv.Name = req.Name;
            inv.Location = req.Location;
            inv.UpdateByUserId = _currentUserService.UserId;

            bool ok = await _inventoryRepo.UpdateAsync(inv);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update inventory", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteInventoryAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            Inventory? inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
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
            Inventory? inv = await _inventoryRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<InventoryRes?>.Failure("Not found", 404);

            InventoryRes res = _mapper.Map<InventoryRes>(inv);
            return GeneralResponse<InventoryRes?>.Success(res, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<InventoryRes>>> GetAllAsync(GetAllInventoryReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<InventoryRes>>.Failure("Unauthorized", 401);

            PagedResult<Inventory> page = await _inventoryRepo.GetAllAsync(req.PageNumber, req.PageSize, x => 
                x.StoreId == _currentUserService.StoreId.Value &&
                (string.IsNullOrEmpty(req.Name) || x.Name.Contains(req.Name)) &&
                (string.IsNullOrEmpty(req.Location) || (x.Location != null && x.Location.Contains(req.Location))));

            PagedResult<InventoryRes> mapped = new ()
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
