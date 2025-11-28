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

namespace StoreSystem.Application.Services.StockMovementService
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly ILogger<StockMovementService> _logger;
        private readonly IMapper _mapper;
        private readonly StockMovementValidator _validator;

        public StockMovementService(IRepository<Product> productRepo,
            IMapper mapper, StockMovementValidator validator, ILogger<StockMovementService> logger, IRepository<StockMovement> movementRepo)
        {
            _movementRepo = movementRepo;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
            _productRepo = productRepo;
        }

        public async Task<GeneralResponse<int>> AddMovementAsync(StockMovementReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = new StockMovement
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

            // update product aggregated stock
            var product = await _productRepo.FindAsync(p => p.Id == req.ProductId);
            if (product != null)
            {
                if (req.Type == BookingSystem.Core.enums.MovementType.In)
                    product.StockQuantity += req.Qty;
                else if (req.Type == BookingSystem.Core.enums.MovementType.Out)
                    product.StockQuantity -= req.Qty;
                else // Adjustment
                    product.StockQuantity = product.StockQuantity + req.Qty; // assume qty can be negative/positive

                await _productRepo.UpdateAsync(product);
                await _productRepo.SaveAsync();
            }

            // publish domain event (if needed, using IMediator in future)

            return GeneralResponse<int>.Success(entity.Id, "Movement added", 201);
        }

        public async Task<GeneralResponse<PagedResult<StockMovementRes>>> GetMovementsByProductAsync(int productId, int pageNumber, int pageSize)
        {
            var page = await _movementRepo.GetAllAsync(pageNumber, pageSize, m => m.ProductId == productId, q => q.OrderByDescending(x => x.Date));
            var mapped = new PagedResult<StockMovementRes>
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
            var page = await _movementRepo.GetAllAsync(1, 1, m => m.ProductId == productId, q => q.OrderByDescending(x => x.Date));
            var last = page.Items.FirstOrDefault();
            if (last == null) return GeneralResponse<bool?>.Failure("No movements", 404);

            // revert product stock
            var product = await _productRepo.FindAsync(p => p.Id == productId);
            if (product != null)
            {
                if (last.Type == BookingSystem.Core.enums.MovementType.In)
                    product.StockQuantity -= last.Qty;
                else if (last.Type == BookingSystem.Core.enums.MovementType.Out)
                    product.StockQuantity += last.Qty;
                else
                    product.StockQuantity -= last.Qty; // best-effort revert

                await _productRepo.UpdateAsync(product);
                await _productRepo.SaveAsync();
            }

            _movementRepo.DeleteAsync(last);
            await _movementRepo.SaveAsync();

            return GeneralResponse<bool?>.Success(true, "Undone", 200);
        }
    }
}