using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using System.Threading.Tasks;
using BookingSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.PaymentContract.Req;
using StoreSystem.Application.Contract.PaymentContract.Res;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Services.PaymentService
{
    /// <summary>
    /// Payment service implementation.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;

        public PaymentService(IRepository<Payment> paymentRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<int>> RecordPaymentAsync(PaymentReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = _mapper.Map<Payment>(req);
            await _paymentRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.Publish(new StoreSystem.Core.Events.PaymentEvent.PaymentProcessedEvent(entity.Id, entity.Amount, entity.CustomerId, entity.SupplierId));

            return GeneralResponse<int>.Success(entity.Id, "Payment recorded", 201);
        }

        public async Task<GeneralResponse<PagedResult<PaymentRes>>> GetPaymentsByCustomerAsync(int customerId, int pageNumber, int pageSize)
        {
            var page = await _paymentRepo.GetAllAsync(pageNumber, pageSize, p => p.CustomerId == customerId);
            var mapped = new PagedResult<PaymentRes>
            {
                Items = page.Items.Select(i => _mapper.Map<PaymentRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<PaymentRes>>.Success(mapped, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<PaymentRes>>> GetPaymentsBySupplierAsync(int supplierId, int pageNumber, int pageSize)
        {
            var page = await _paymentRepo.GetAllAsync(pageNumber, pageSize, p => p.SupplierId == supplierId);
            var mapped = new PagedResult<PaymentRes>
            {
                Items = page.Items.Select(i => _mapper.Map<PaymentRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<PaymentRes>>.Success(mapped, "Ok", 200);
        }
    }
}
