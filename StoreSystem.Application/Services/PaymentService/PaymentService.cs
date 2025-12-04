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
using StoreSystem.Core.Events.PaymentEvent;
using AutoMapper;
using FluentValidation;

namespace StoreSystem.Application.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IUniteOfWork _uow;
        private readonly IEventBus _mediator;
        private readonly IMapper _mapper;
        private readonly IValidator<PaymentReq> _Validator;
        private readonly ICurrentUserService _currentUserService;

        public PaymentService(IValidator<PaymentReq> Validator,IRepository<Payment> paymentRepo, IUniteOfWork uow, IEventBus mediator, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _Validator = Validator;
            _paymentRepo = paymentRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> RecordPaymentAsync(PaymentReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var Result = await ValidateRequest.IsValid<PaymentReq>(_Validator, req);
            if (!Result.Item1) return GeneralResponse<int>.Failure(Result.Item2, 400);
            
            Payment entity = _mapper.Map<Payment>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _paymentRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new PaymentProcessedEvent(entity.Id, entity.Amount, entity.CustomerId, entity.SupplierId));

            return GeneralResponse<int>.Success(entity.Id, "Payment recorded", 201);
        }

        public async Task<GeneralResponse<PagedResult<PaymentRes>>> GetPaymentsByCustomerAsync(int customerId, int pageNumber, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<PaymentRes>>.Failure("Unauthorized", 401);

            var page = await _paymentRepo.GetAllAsync(pageNumber, pageSize, p => p.CustomerId == customerId && p.StoreId == _currentUserService.StoreId.Value);
            PagedResult<PaymentRes> mapped = new ()
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
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<PaymentRes>>.Failure("Unauthorized", 401);

            PagedResult<Payment> page = await _paymentRepo.GetAllAsync(pageNumber, pageSize, p => p.SupplierId == supplierId && p.StoreId == _currentUserService.StoreId.Value);
            PagedResult<PaymentRes> mapped = new ()
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
