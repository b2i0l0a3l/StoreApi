using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.EmployeeContract.Req;

namespace StoreSystem.Application.Contract.EmployeeContract.Validator
{
    public class EmployeeValidator : AbstractValidator<EmployeeReq>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty().GreaterThan(1).WithMessage("Store Id is Required");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is Required");
        }
    }
}