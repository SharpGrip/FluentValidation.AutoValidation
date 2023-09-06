using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Results
{
    public class FluentValidationAutoValidationDefaultResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
        {
            return new BadRequestObjectResult(validationProblemDetails);
        }
    }
}