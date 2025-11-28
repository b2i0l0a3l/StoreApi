using StoreSystem.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.PaymentEvent;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Events.Handlers
{
    public class PaymentProcessedEventHandler : INotificationHandler<PaymentProcessedEvent>
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<SalesInvoice> _salesRepo;
        private readonly IRepository<PurchaseInvoice> _purchaseRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;

        public PaymentProcessedEventHandler(IRepository<Payment> paymentRepo,
            IRepository<SalesInvoice> salesRepo,
            IRepository<PurchaseInvoice> purchaseRepo,
            IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo,
            IUniteOfWork uow)
        {
            _paymentRepo = paymentRepo;
            _salesRepo = salesRepo;
            _purchaseRepo = purchaseRepo;
            _eventRepo = eventRepo;
            _uow = uow;
        }

        public async Task Handle(PaymentProcessedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(PaymentProcessedEvent) && e.AggregateId == notification.PaymentId.ToString());
                if (existing != null) return;

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent
                {
                    EventType = nameof(PaymentProcessedEvent),
                    AggregateId = notification.PaymentId.ToString(),
                    ProcessedAt = DateTime.UtcNow
                });
                var payment = await _paymentRepo.FindAsync(p => p.Id == notification.PaymentId);
                if (payment == null) return;

                var sale = await _salesRepo.FindAsync(s => s.Id == payment.InvoiceId);
                if (sale != null)
                {
                    sale.PaidAmount += notification.Amount;
                    sale.DueAmount = sale.TotalAmount - sale.PaidAmount;
                    await _salesRepo.UpdateAsync(sale);
                    await _uow.CompleteAsync();
                    return;
                }

                // try purchase invoice
                var purchase = await _purchaseRepo.FindAsync(p => p.Id == payment.InvoiceId);
                if (purchase != null)
                {
                    purchase.PaidAmount += notification.Amount;
                    purchase.DueAmount = purchase.TotalAmount - purchase.PaidAmount;
                    await _purchaseRepo.UpdateAsync(purchase);
                    await _uow.CompleteAsync();
                }
            }
            catch
            {
            }
        }
    }
}

