using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Categories.req;
using StoreSystem.Core.Entities;
using FluentValidation;

namespace StoreSystem.Application.Contract.Categories.Validator
{
    public class CategoryValidator : AbstractValidator<CategoryReq>

    {
            public CategoryValidator()
            {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is Required")
                .MaximumLength(30);

            }

        
    }
}