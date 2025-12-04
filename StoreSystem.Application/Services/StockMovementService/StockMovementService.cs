using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using AutoMapper;
using StoreSystem.Core.Entities;
using Microsoft.Extensions.Logging;
using StoreSystem.Application.Contract.StockMovementContract.req;
using StoreSystem.Application.Contract.StockMovementContract.res;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.StockMovementContract.validator;
using StoreSystem.Application.Interfaces;
using FluentValidation;
using BookingSystem.Core.enums;

namespace StoreSystem.Application.Services.StockMovementService
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly ILogger<StockMovementService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<StockMovementReq> _validator;
        private readonly ICurrentUserService _currentUserService;
        private enum enMode {Undo, handling};
        private enMode _Mode { get; set; } = enMode.handling;

        public StockMovementService(IRepository<Product> productRepo,
            IMapper mapper, IValidator<StockMovementReq> validator, ILogger<StockMovementService> logger, IRepository<StockMovement> movementRepo, ICurrentUserService currentUserService)
        {
            _movementRepo = movementRepo;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
            _productRepo = productRepo;
            _currentUserService = currentUserService;
        }
    
        public async Task<GeneralResponse<int>> AddMovementAsync(StockMovementReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            Product? product = await _productRepo.FindAsync(p => p.Id == req.ProductId && p.StoreId == _currentUserService.StoreId.Value);
            if (product == null) return GeneralResponse<int>.Failure("Product not found", 404);

            StockMovement entity = new()
            {
                ProductId = req.ProductId,
                InventoryId = req.InventoryId,
                Qty = req.Qty,
                Type = req.Type,
                ReferenceId = req.ReferenceId,
                Note = req.Note,
                Date = req.Date
            };

            await _movementRepo.AddAsync(entity);
            await _movementRepo.SaveAsync();

            ProcessStockMovement(entity,  product);
            await _productRepo.UpdateAsync(product);
            await _productRepo.SaveAsync();


            return GeneralResponse<int>.Success(entity.Id, "Movement added", 201);
        }

        public async Task<GeneralResponse<PagedResult<StockMovementRes>>> GetMovementsByProductAsync(int productId, int pageNumber, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<StockMovementRes>>.Failure("Unauthorized", 401);

            Product? product = await _productRepo.FindAsync(p => p.Id == productId && p.StoreId == _currentUserService.StoreId.Value);
            if (product == null) return GeneralResponse<PagedResult<StockMovementRes>>.Failure("Product not found", 404);

            PagedResult<StockMovement> page = await _movementRepo.GetAllAsync(pageNumber, pageSize, m => m.ProductId == productId, q => q.OrderByDescending(x => x.Date));
            PagedResult<StockMovementRes> mapped = new ()
            {
                Items = page.Items.Select(m => new StockMovementRes
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    InventoryId = m.InventoryId,
                    Qty = m.Qty,
                    Type = m.Type,
                    ReferenceId = m.ReferenceId,
                    Date = m.Date,
                    Note = m.Note
                }),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };

            return GeneralResponse<PagedResult<StockMovementRes>>.Success(mapped, "Ok", 200);
        }

        public async Task<GeneralResponse<bool?>> UndoLastMovementAsync(int productId)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            Product? product = await _productRepo.FindAsync(p => p.Id == productId && p.StoreId == _currentUserService.StoreId.Value);
            if (product == null) return GeneralResponse<bool?>.Failure("Product not found", 404);

            PagedResult<StockMovement> page = await _movementRepo.GetAllAsync(1, 1, m => m.ProductId == productId && m.Inventory!.StoreId == _currentUserService.StoreId.Value, q => q.OrderByDescending(x => x.Date));
            StockMovement? last = page.Items.FirstOrDefault();
            if (last == null) return GeneralResponse<bool?>.Failure("No movements", 404);

            ProcessStockMovement(last, product, enMode.Undo);

            await _productRepo.UpdateAsync(product);
            await _productRepo.SaveAsync();

            _movementRepo.DeleteAsync(last);
            await _movementRepo.SaveAsync();

            return GeneralResponse<bool?>.Success(true, "Undone", 200);
        }

        
            private void RevertStockMovement(StockMovement last, Product product)
        {
             if (last.Type == MovementType.In)
                product.StockQuantity -= last.Qty;
            else if (last.Type == MovementType.Out)
                product.StockQuantity += last.Qty;
            else
                product.StockQuantity -= last.Qty; 
        }
        private void ApplyStockMovement(StockMovement req,  Product product)
        {

            if (req.Type == MovementType.In)
                product.StockQuantity += req.Qty;
            else if (req.Type == MovementType.Out)
                product.StockQuantity -= req.Qty;
            else
                product.StockQuantity = product.StockQuantity + req.Qty;
        }
        private void ProcessStockMovement(StockMovement entity,  Product product,enMode Mode = enMode.handling)
        {
            if (Mode == enMode.Undo)
                RevertStockMovement(entity,  product);
            else
                ApplyStockMovement(entity,  product);
        }
        
    }
}