using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.SaleContract.Req;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Application.Services.SaleService
{
    public class SalesInvoiceService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<SalesInvoice> _Repo;
        public SalesInvoiceService(IRepository<SalesInvoice> Repo ,ICurrentUserService CurrentUserService)
        {
            _currentUserService = CurrentUserService;
            _Repo = Repo;
        }
        public async Task<int> AddNewSaleInvoice(SaleReq req)
        {
            SalesInvoice invoice = new ()
            {
                CustomerId = req.CustomerId ,
                Date = req.Date,
                StoreId = _currentUserService.StoreId!.Value,
                Status = BookingSystem.Core.enums.InvoiceStatus.Pending,
                CreateByUserId = _currentUserService.UserId,
                UpdateByUserId = _currentUserService.UserId
            };
            await _Repo.AddAsync(invoice);
            return invoice.Id;
        }
    }
}