using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.StockContract.validator
{
    public class StockValidator : AbstractValidator<Stock>
    {
        public StockValidator()
        {
            RuleFor(x => x.InventoryId).NotEmpty().GreaterThan(0).WithMessage("Inventory Id is Required");
            RuleFor(x => x.ProductId).NotEmpty().GreaterThan(0).WithMessage("Product Id is Required");
        }
        
    }
}