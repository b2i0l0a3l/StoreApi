using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.CustomerContract.Req;
using StoreSystem.Application.Contract.CustomerContract.Res;
using StoreSystem.Application.Interfaces;
using AutoMapper;
using FluentValidation;
using StoreSystem.Core.Events.CustomerEvent;

namespace StoreSystem.Application.Services.CustomerService
{

    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly IValidator<CustomerReq> _Validator;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CustomerService(IValidator<CustomerReq> Validator,IRepository<Customer> customerRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _customerRepo = customerRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _Validator = Validator;
        }

        public async Task<GeneralResponse<int>> CreateCustomerAsync(CustomerReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var result = await ValidateRequest.IsValid<CustomerReq>(_Validator,req);
            if (!result.Item1)
            {
                return GeneralResponse<int>.Failure(string.Join(" ,", result.Item2));
            }

            var entity = _mapper.Map<Customer>(req);
            entity.StoreId = _currentUserService.StoreId.Value;
            entity.CreateByUserId = _currentUserService.UserId;
            entity.UpdateByUserId = _currentUserService.UserId;

            await _customerRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.Publish(new CustomerCreatedEvent(entity.Id, entity.Name, DateTime.UtcNow));

            return GeneralResponse<int>.Success(entity.Id, "Customer created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateCustomerAsync(int id, CustomerReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);

            var result = await ValidateRequest.IsValid<CustomerReq>(_Validator,req);
            if (!result.Item1)
            {
                return GeneralResponse<bool?>.Failure(string.Join(" ,", result.Item2));
            }
            
            var c = await _customerRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (c == null) return GeneralResponse<bool?>.Failure("Customer not found", 404);

            c.Name = req.Name;
            c.Phone = req.Phone;
            c.Email = req.Email;
            c.UpdateByUserId = _currentUserService.UserId;

            var ok = await _customerRepo.UpdateAsync(c);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update customer", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteCustomerAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var c = await _customerRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (c == null) return GeneralResponse<bool?>.Failure("Customer not found", 404);

            _customerRepo.DeleteAsync(c);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<CustomerRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<CustomerRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<CustomerRes?>.Failure("Invalid id", 400);
            var c = await _customerRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (c == null) return GeneralResponse<CustomerRes?>.Failure("Not found", 404);
            return GeneralResponse<CustomerRes?>.Success(_mapper.Map<CustomerRes>(c), "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<CustomerRes>>> GetAllAsync(GetCustomerReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<CustomerRes>>.Failure("Unauthorized", 401);

            var page = await _customerRepo.GetAllAsync(req.PageNumber, req.PageSize, x => 
                x.StoreId == _currentUserService.StoreId.Value &&
                (string.IsNullOrEmpty(req.Name) || x.Name.Contains(req.Name)) &&
                (string.IsNullOrEmpty(req.Phone) || (x.Phone != null && x.Phone.Contains(req.Phone))));

            var mapped = new PagedResult<CustomerRes>
            {
                Items = page.Items.Select(i => _mapper.Map<CustomerRes>(i)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<CustomerRes>>.Success(mapped, "Ok", 200);
        }
    }
}
