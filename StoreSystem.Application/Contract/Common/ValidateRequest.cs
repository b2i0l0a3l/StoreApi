using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace StoreSystem.Application.Contract.Common
{
    public static class ValidateRequest
    {
        public  static async Task<(bool,string)> IsValid<T>(IValidator<T> Validator,T model)
        {
            var result = await Validator.ValidateAsync(model);
            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                return  (false, errors);
            }
            return (true, "");
        }
    }
}