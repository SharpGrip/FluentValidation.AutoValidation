using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class ValidationResultExtensions
    {
        public static IDictionary<string, string[]> ToValidationProblemErrors(this ValidationResult validationResult)
        {
            return validationResult.Errors.GroupBy(validationFailure => validationFailure.PropertyName)
                .ToDictionary(validationFailureGrouping => validationFailureGrouping.Key,
                    validationFailureGrouping => validationFailureGrouping.Select(validationFailure => validationFailure.ErrorMessage).ToArray());
        }
    }
}