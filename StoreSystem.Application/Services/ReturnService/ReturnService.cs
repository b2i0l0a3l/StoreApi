using StoreSystem.Core.Interfaces;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.ReturnContract.Req;
using StoreSystem.Application.Interfaces;
using MediatR;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Events.SaleEvent;
using StoreSystem.Core.Events.PurchaseEvent;

namespace StoreSystem.Application.Services.ReturnService
{
    /// <summary>
    /// Handles sales and purchase returns with store-level isolation.
    /// </summary>
    public class ReturnService : IReturnService
    {
        private readonly IRepository<SalesInvoice> _salesRepo;
        private readonly IRepository<PurchaseInvoice> _purchaseRepo;
        private readonly IEventBus _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ReturnService(
            IRepository<SalesInvoice> salesRepo,
            IRepository<PurchaseInvoice> purchaseRepo,
            IEventBus mediator,
            ICurrentUserService currentUserService)
        {
            _salesRepo = salesRepo;
            _purchaseRepo = purchaseRepo;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreateSalesReturnAsync(SalesReturnReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var sale = await _salesRepo.FindAsync(x => x.Id == req.SaleId && x.StoreId == _currentUserService.StoreId.Value);
            if (sale == null) return GeneralResponse<int>.Failure("Sale not found", 404);

            await _mediator.PublishAsync(new SalesReturnCreatedEvent(0, req.SaleId, req.Date));
            return GeneralResponse<int>.Success(0, "Sales return processed (event published)", 200);
        }

        public async Task<GeneralResponse<int>> CreatePurchaseReturnAsync(PurchaseReturnReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var purchase = await _purchaseRepo.FindAsync(x => x.Id == req.PurchaseId && x.StoreId == _currentUserService.StoreId.Value);
            if (purchase == null) return GeneralResponse<int>.Failure("Purchase not found", 404);

            await _mediator.PublishAsync(new PurchaseReturnCreatedEvent(0, req.PurchaseId, req.Date));
            return GeneralResponse<int>.Success(0, "Purchase return processed (event published)", 200);
        }
    }
}
