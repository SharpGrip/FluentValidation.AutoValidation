using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Results
{
    public class FluentValidationAutoValidationDefaultResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults)
        {
            return Task.FromResult<IActionResult?>(new BadRequestObjectResult(validationProblemDetails));
        }
    }
}