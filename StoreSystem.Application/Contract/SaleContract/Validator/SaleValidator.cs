using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.SaleContract.Req;

namespace StoreSystem.Application.Contract.SaleContract.Validator
{
    public class SaleValidator : AbstractValidator<SaleReq>
    {
        public SaleValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Customer Id Is Invalid");
            RuleFor(x => x.CustomerId).Empty().WithMessage("Customer Id Is Required");
            RuleFor(x => x.Items).Empty().WithMessage("Items are Required");

        }
    }
}