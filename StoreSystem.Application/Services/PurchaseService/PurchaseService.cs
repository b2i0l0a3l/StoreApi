using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using MediatR;
using StoreSystem.Application.Contract.PurchaseContract.Req;
using StoreSystem.Application.Contract.PurchaseContract.Res;
using StoreSystem.Application.Contract.ReturnContract.Req;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Events.PurchaseEvent;

namespace StoreSystem.Application.Services.PurchaseService
{

    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<PurchaseInvoice> _purchaseRepo;
        private readonly IUniteOfWork _uow;
        private readonly IEventBus _mediator;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public PurchaseService(IRepository<PurchaseInvoice> purchaseRepo, IUniteOfWork uow, IEventBus mediator, AutoMapper.IMapper mapper, ICurrentUserService currentUserService)
        {
            _purchaseRepo = purchaseRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> CreatePurchaseAsync(PurchaseReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            PurchaseInvoice invoice = new ()
            {
                SupplierId = req.SupplierId,
                Date = req.Date,
                StoreId = _currentUserService.StoreId.Value,
                Status = BookingSystem.Core.enums.InvoiceStatus.Pending,
                CreateByUserId = _currentUserService.UserId,
                UpdateByUserId = _currentUserService.UserId,
                PurchaseItems = req.Items.Select
                (item=> new PurchaseItem
                {
                    ProductId = item.ProductId,
                    Qty = item.Quantity,
                    CostPrice = item.UnitCost,
                    Total = item.Quantity * item.UnitCost
                      }).ToList()
             };


            invoice.TotalAmount = invoice.PurchaseItems.Sum(p => p.Total);
            invoice.PaidAmount = 0;
            invoice.DueAmount = invoice.TotalAmount;

            await _purchaseRepo.AddAsync(invoice);
            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new PurchaseCreatedEvent(invoice.Id, invoice.StoreId, invoice.Date));

            return GeneralResponse<int>.Success(invoice.Id, "Purchase created", 201);
        }

        public async Task<GeneralResponse<PurchaseRes?>> GetByIdAsync(int id)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PurchaseRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<PurchaseRes?>.Failure("Invalid id", 400);
            PurchaseInvoice? inv = await _purchaseRepo.FindAsync(x => x.Id == id && x.StoreId == _currentUserService.StoreId.Value);
            if (inv == null) return GeneralResponse<PurchaseRes?>.Failure("Not found", 404);

            PurchaseRes res = new()
            {
                Id = inv.Id,
                SupplierId = inv.SupplierId,
                Date = inv.Date,
                Items = inv.PurchaseItems.Select(p => new PurchaseItemRes
                {
                    ProductId = p.ProductId,
                    Quantity = p.Qty,
                    UnitCost = p.CostPrice
                }).ToArray()
            };
            
            return GeneralResponse<PurchaseRes?>.Success(res, "Ok", 200);
        }

        public async Task<GeneralResponse<PagedResult<PurchaseRes>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<PagedResult<PurchaseRes>>.Failure("Unauthorized", 401);

            PagedResult<PurchaseInvoice> page = await _purchaseRepo.GetAllAsync(pageNumber, pageSize, x => x.StoreId == _currentUserService.StoreId.Value);
            PagedResult<PurchaseRes> mapped = new ()
            {
                Items = page.Items.Select(inv => new PurchaseRes
                {
                    Id = inv.Id,
                    SupplierId = inv.SupplierId,
                    Date = inv.Date,
                    Items = inv.PurchaseItems.Select(p => new PurchaseItemRes { ProductId = p.ProductId, Quantity = p.Qty, UnitCost = p.CostPrice })
                }),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };
            return GeneralResponse<PagedResult<PurchaseRes>>.Success(mapped, "Ok", 200);
        }

        public async Task<GeneralResponse<int>> ReturnPurchaseAsync(PurchaseReturnReq req)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.StoreId.HasValue)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var purchase = await _purchaseRepo.FindAsync(x => x.Id == req.PurchaseId && x.StoreId == _currentUserService.StoreId.Value);
            if (purchase == null) return GeneralResponse<int>.Failure("Purchase not found", 404);

            await _mediator.PublishAsync(new PurchaseReturnCreatedEvent(0, req.PurchaseId, req.Date));
            return GeneralResponse<int>.Success(0, "Return processed (event published)", 200);
        }
    }
}
