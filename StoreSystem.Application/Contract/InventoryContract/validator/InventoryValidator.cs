using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StoreSystem.Application.Contract.InventoryContract.req;

namespace StoreSystem.Application.Contract.InventoryContract.validator
{
    public class InventoryValidator : AbstractValidator<InventoryReq>
    {
        public InventoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Is Required!");
            RuleFor(x => x.StoreId).NotEmpty().GreaterThan(0).WithMessage("Store Is Required!");
        }
    }
}