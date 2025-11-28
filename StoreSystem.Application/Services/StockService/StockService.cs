using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.StockContract.req;
using StoreSystem.Application.Events;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Services.StockService
{
    /// <summary>
    /// Stock management service. Handles create/increase/decrease/adjust operations and publishes events on changes.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<Product> _productRepo;

        private readonly IUniteOfWork _uow;
        private readonly IEventBus _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private ICurrentUserService _CurrentUserService;

        public StockService(ICurrentUserService currentUserService,IRepository<Stock> stockRepo, IRepository<Product> productRepo, IUniteOfWork uow, IEventBus mediator, AutoMapper.IMapper mapper)
        {
            _stockRepo = stockRepo;
            _productRepo = productRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _CurrentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreateStockAsync(StockReq req)
        {
            if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);
            var entity = _mapper.Map<Stock>(req);
            entity.LastUpdated = DateTime.UtcNow;
            await _stockRepo.AddAsync(entity);
            await _uow.CompleteAsync();
            await _mediator.PublishAsync(new StockChangedEvent(entity.ProductId, entity.InventoryId, entity.Quantity));

            return GeneralResponse<int>.Success(entity.Id, "Stock created", 201);
        }

        public async Task<GeneralResponse<int>> IncreaseStockAsync(StockReq req)
        {
            if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var stock = await _stockRepo.FindAsync(s => s.ProductId == req.ProductId && s.InventoryId == _CurrentUserService.InventoryId);
            if (stock == null)
            {
                return await CreateStockAsync(req);
            }

            stock.Quantity += req.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            await _stockRepo.UpdateAsync(stock);
            await _uow.CompleteAsync();

            var product = await _productRepo.FindAsync(p => p.Id == req.ProductId);
            if (product != null)
            {
                product.StockQuantity += (int)req.Quantity;
                await _productRepo.UpdateAsync(product);
            }

            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new StockChangedEvent(req.ProductId,stock.InventoryId, req.Quantity));

            return GeneralResponse<int>.Success((int)stock.Quantity, "Stock increased", 200);
        }

        public async Task<GeneralResponse<int>> DecreaseStockAsync(StockReq req)
        {
             if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var stock = await _stockRepo.FindAsync(s => s.ProductId == req.ProductId && s.InventoryId == _CurrentUserService.InventoryId);
            if (stock == null) return GeneralResponse<int>.Failure("Stock not found", 404);

            if (stock.Quantity < req.Quantity) return GeneralResponse<int>.Failure("Insufficient stock", 400);

            stock.Quantity -= req.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            await _stockRepo.UpdateAsync(stock);
            await _uow.CompleteAsync();

            var product = await _productRepo.FindAsync(p => p.Id == req.ProductId);
            if (product != null)
            {
                product.StockQuantity -= (int)req.Quantity;
                await _productRepo.UpdateAsync(product);
            }

            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new StockChangedEvent(req.ProductId, stock.InventoryId, -req.Quantity));

            return GeneralResponse<int>.Success((int)stock.Quantity, "Stock decreased", 200);
        }

        public async Task<GeneralResponse<int>> AdjustStockAsync(StockReq req)
        {
            if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var stock = await _stockRepo.FindAsync(s => s.ProductId == req.ProductId && s.InventoryId == _CurrentUserService.InventoryId);
            if (stock == null) return GeneralResponse<int>.Failure("Stock not found", 404);

            var delta = req.Quantity - stock.Quantity;
            stock.Quantity = req.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            await _stockRepo.UpdateAsync(stock);
            await _uow.CompleteAsync();

            var product = await _productRepo.FindAsync(p => p.Id == req.ProductId);
            if (product != null)
            {
                product.StockQuantity += (int)delta;
                await _productRepo.UpdateAsync(product);
            }

            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new StockChangedEvent(req.ProductId, stock.InventoryId, delta));

            return GeneralResponse<int>.Success((int)stock.Quantity, "Stock adjusted", 200);
        }

        public async Task<GeneralResponse<int>> GetCurrentStockAsync(StockReq req)
        {
             if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);
            var stock = await _stockRepo.FindAsync(s => s.ProductId == req.ProductId && s.InventoryId == _CurrentUserService.InventoryId );
            if (stock == null) return GeneralResponse<int>.Success(0, "No stock", 200);
            return GeneralResponse<int>.Success((int)stock.Quantity, "Ok", 200);
        }

        public async Task<GeneralResponse<int>> GetLowStockProductsAsync(StockReq req)
        {
             if (IsUserAuthorized())
            {
                return GeneralResponse<int>.Failure("Unauthorized", 403);
            }
            
            var all = await _productRepo.GetAllAsync(1, int.MaxValue);
            var lowCount = all.Items.Count(p => p.StockQuantity < req.Quantity);
            return GeneralResponse<int>.Success(lowCount, "Ok", 200);
        }

        private bool IsUserAuthorized()
        {
            if (_CurrentUserService.UserId == null || _CurrentUserService.InventoryId == null)
                return false;
            return true;
        }
  
    }
}
