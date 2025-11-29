using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.CustomerContract.Req;

namespace StoreSystem.Application.Contract.CustomerContract.validator
{
    public class CustomerValidator : AbstractValidator<CustomerReq>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required");
            
        }
    }
}