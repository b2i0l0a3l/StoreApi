using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.CustomerContract.Req;
using StoreSystem.Application.Contract.CustomerContract.Res;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Services.CustomerService
{
    /// <summary>
    /// Customer service implementation.
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;

        public CustomerService(IRepository<Customer> customerRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper)
        {
            _customerRepo = customerRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<int>> CreateCustomerAsync(CustomerReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var entity = _mapper.Map<Customer>(req);
            await _customerRepo.AddAsync(entity);
            await _uow.CompleteAsync();

            await _mediator.Publish(new StoreSystem.Core.Events.CustomerEvent.CustomerCreatedEvent(entity.Id, entity.Name, DateTime.UtcNow));

            return GeneralResponse<int>.Success(entity.Id, "Customer created", 201);
        }

        public async Task<GeneralResponse<bool?>> UpdateCustomerAsync(int id, CustomerReq req)
        {
            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);
            var c = await _customerRepo.FindAsync(x => x.Id == id);
            if (c == null) return GeneralResponse<bool?>.Failure("Customer not found", 404);

            c.Name = req.Name;
            c.Phone = req.Phone;
            c.Email = req.Email;

            var ok = await _customerRepo.UpdateAsync(c);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update customer", 500);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Updated", 200);
        }

        public async Task<GeneralResponse<bool?>> DeleteCustomerAsync(int id)
        {
            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var c = await _customerRepo.FindAsync(x => x.Id == id);
            if (c == null) return GeneralResponse<bool?>.Failure("Customer not found", 404);

            _customerRepo.DeleteAsync(c);
            await _uow.CompleteAsync();
            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }

        public async Task<GeneralResponse<CustomerRes?>> GetByIdAsync(int id)
        {
            if (id < 1) return GeneralResponse<CustomerRes?>.Failure("Invalid id", 400);
            var c = await _customerRepo.FindAsync(x => x.Id == id);
            if (c == null) return GeneralResponse<CustomerRes?>.Failure("Not found", 404);
            return GeneralResponse<CustomerRes?>.Success(_mapper.Map<CustomerRes>(c), "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<CustomerRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var page = await _customerRepo.GetAllAsync(pageNumber, pageSize);
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
