using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.PaymentContract.Req;

namespace StoreSystem.Application.Contract.PaymentContract.Validator
{
    public class PaymentValidator : AbstractValidator<PaymentReq>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Amount).NotEmpty().GreaterThan(0).WithMessage("Amount Is Required");
        }
        
    }
}