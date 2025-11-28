using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.StoreContract.req;

namespace StoreSystem.Application.Contract.StoreContract.validator
{
    public class StoreValidator : AbstractValidator<StoreReq>
    {
        public StoreValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required For Store");
        }
    }
}