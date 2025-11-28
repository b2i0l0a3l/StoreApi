using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.StockMovementContract.req;

namespace StoreSystem.Application.Contract.StockMovementContract.validator
{
    public class StockMovementValidator : AbstractValidator<StockMovementReq>
    {
        public StockMovementValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().GreaterThan(0).WithMessage("Product Id is Required");
            RuleFor(x => x.InventoryId).NotEmpty().GreaterThan(0).WithMessage("Inventory Id is Required");
            RuleFor(x => x.Qty).NotEmpty().GreaterThan(0).WithMessage("Quantity should be greath than 0");
        }
    }
}