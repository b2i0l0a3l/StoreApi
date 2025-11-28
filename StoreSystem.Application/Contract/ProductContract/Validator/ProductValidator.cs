using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.ProductContract.Req;
using FluentValidation;

namespace StoreSystem.Application.Contract.ProductContract.Validator
{
    public class ProductValidator : AbstractValidator<ProductReq>
    {

        public ProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name Is Required!");
            RuleFor(p => p.StoreId).NotEmpty().WithMessage("Store Id Is Required!");
            RuleFor(p => p.CategoryId).NotEmpty().WithMessage("Category  Id Is Required!");
            RuleFor(p => p.CostPrice).GreaterThan(0).WithMessage("Cost price Is Required!");
            RuleFor(p => p.SellPrice).GreaterThan(0).WithMessage("sell Price Is Required!");
            RuleFor(p => p.StockQuantity).GreaterThan(0).WithMessage("Stock Quantity Is Required!");
        }
        
    }
}