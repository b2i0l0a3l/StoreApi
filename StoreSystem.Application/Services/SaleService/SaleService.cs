using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.SaleContract.Req;
using StoreSystem.Application.Contract.SaleContract.Res;
using StoreSystem.Application.Interfaces;
using FluentValidation;
using StoreSystem.Application.Contract.ReturnContract.Req;
using StoreSystem.Core.Events.SaleEvent;

namespace StoreSystem.Application.Services.SaleService
{

    public class SaleService : ISaleService
    {
        private readonly IRepository<SalesInvoice> _saleRepo;
        private readonly IUniteOfWork _uow;
        private readonly IEventBus _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IValidator<SaleReq> _Validator;
        private readonly ICurrentUserService _currentUserService;

        public SaleService(IValidator<SaleReq> Validator,IRepository<SalesInvoice> saleRepo, IUniteOfWork uow, IEventBus mediator, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _saleRepo = saleRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _Validator = Validator;
        }

        public async Task<GeneralResponse<int>> CreateSaleAsync(SaleReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);
           
            var result = await ValidateRequest.IsValid<SaleReq>(_Validator,req);
            if (!result.Item1)
            {
                return GeneralResponse<int>.Failure(string.Join(" ,", result.Item2));
            }
            
            var invoice = new SalesInvoice
            {
                CustomerId = req.CustomerId,
                Date = req.Date,
                StoreId = _currentUserService.StoreId.Value,
                Status = BookingSystem.Core.enums.InvoiceStatus.Pending,
                CreateByUserId = _currentUserService.UserId,
                UpdateByUserId = _currentUserService.UserId
            };

            foreach (var item in req.Items)
            {
                var si = new SalesItem
                {
                    ProductId = item.ProductId,
                    Qty = item.Quantity,
                    SellPrice = item.UnitPrice,
                    Total = item.Quantity * item.UnitPrice
                };
                invoice.SalesItems.Add(si);
            }

            invoice.TotalAmount = invoice.SalesItems.Sum(s => s.Total);
            invoice.PaidAmount = 0;
            invoice.DueAmount = invoice.TotalAmount;

            await _saleRepo.AddAsync(invoice);
            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new SaleCreatedEvent(invoice.Id, invoice.StoreId, invoice.Date));

            return GeneralResponse<int>.Success(invoice.Id, "Sale created", 201);
        }

        public async Task<GeneralResponse<SaleRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<SaleRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<SaleRes?>.Failure("Invalid id", 400);
            var inv = await _saleRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<SaleRes?>.Failure("Not found", 404);

            SaleRes res = new ()
            {
                Id = inv.Id,
                StoreId = inv.StoreId,
                CustomerId = inv.CustomerId,
                Date = inv.Date,
                Items = inv.SalesItems.Select(s => new SaleItemRes
                {
                    ProductId = s.ProductId,
                    Quantity = s.Qty,
                    UnitPrice = s.SellPrice
                }).ToList()
            };

            return GeneralResponse<SaleRes?>.Success(res, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<SaleRes>>> GetAllAsync(GetSaleReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<SaleRes>>.Failure("Unauthorized", 401);

            PagedResult<SalesInvoice> page = await _saleRepo.GetAllAsync(req.PageNumber, req.PageSize, x => 
                x.StoreId == _currentUserService.StoreId.Value &&
                (!req.CustomerId.HasValue || x.CustomerId == req.CustomerId) &&
                (!req.FromDate.HasValue || x.Date >= req.FromDate) &&
                (!req.ToDate.HasValue || x.Date <= req.ToDate));

            PagedResult<SaleRes> mapped = new()
            {
                Items = page.Items.Select(inv => new SaleRes
                {
                    Id = inv.Id,
                    StoreId = inv.StoreId,
                    CustomerId = inv.CustomerId,
                    Date = inv.Date,
                    Items = inv.SalesItems.Select(s => new SaleItemRes { ProductId = s.ProductId, Quantity = s.Qty, UnitPrice = s.SellPrice })
                }),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<SaleRes>>.Success(mapped, "Ok", 200);
        }

        public async Task<GeneralResponse<int>> ReturnSaleAsync(SalesReturnReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var sale = await _saleRepo.FindAsync(x => x.Id == req.SaleId && x.StoreId == _currentUserService.StoreId.Value);
            if (sale == null) return GeneralResponse<int>.Failure("Sale not found", 404);

            await _mediator.PublishAsync(new SalesReturnCreatedEvent(0, req.SaleId, req.Date));
            return GeneralResponse<int>.Success(0, "Return processed (event published)", 200);
        }
    }
}
