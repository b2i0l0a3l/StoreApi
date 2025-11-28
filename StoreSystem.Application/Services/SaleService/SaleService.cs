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

namespace StoreSystem.Application.Services.SaleService
{
    /// <summary>
    /// Handles sales creation and queries.
    /// </summary>
    public class SaleService : ISaleService
    {
        private readonly IRepository<SalesInvoice> _saleRepo;
        private readonly IUniteOfWork _uow;
        private readonly IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;

        public SaleService(IRepository<SalesInvoice> saleRepo, IUniteOfWork uow, IMediator mediator, AutoMapper.IMapper mapper)
        {
            _saleRepo = saleRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<int>> CreateSaleAsync(SaleReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var invoice = new SalesInvoice
            {
                CustomerId = req.CustomerId ?? 0,
                Date = req.Date,
                StoreId = req.StoreId,
                Status = BookingSystem.Core.enums.InvoiceStatus.Pending
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

            await _mediator.Publish(new StoreSystem.Core.Events.SaleEvent.SaleCreatedEvent(invoice.Id, invoice.StoreId, invoice.Date));

            return GeneralResponse<int>.Success(invoice.Id, "Sale created", 201);
        }

        public async Task<GeneralResponse<SaleRes?>> GetByIdAsync(int id)
        {
            if (id < 1) return GeneralResponse<SaleRes?>.Failure("Invalid id", 400);
            var inv = await _saleRepo.FindAsync(x => x.Id == id);
            if (inv == null) return GeneralResponse<SaleRes?>.Failure("Not found", 404);

            var res = new SaleRes
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
                }).ToArray()
            };

            return GeneralResponse<SaleRes?>.Success(res, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<SaleRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var page = await _saleRepo.GetAllAsync(pageNumber, pageSize);
            var mapped = new PagedResult<SaleRes>
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

        public async Task<GeneralResponse<int>> ReturnSaleAsync(StoreSystem.Application.Contract.ReturnContract.Req.SalesReturnReq req)
        {
            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            // Create a return record as SalesReturnCreatedEvent; detailed return persistence handled elsewhere
            await _mediator.Publish(new StoreSystem.Core.Events.SaleEvent.SalesReturnCreatedEvent(0, req.SaleId, req.Date));
            return GeneralResponse<int>.Success(0, "Return processed (event published)", 200);
        }
    }
}
