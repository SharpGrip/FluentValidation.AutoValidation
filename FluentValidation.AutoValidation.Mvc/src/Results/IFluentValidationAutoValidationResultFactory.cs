using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Results
{
    public interface IFluentValidationAutoValidationResultFactory
    {
        /// <summary>
        /// Creates a <see cref="IActionResult"/> to be executed by the controller action.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutingContext"/> associated with the current request/response.</param>
        /// <param name="validationProblemDetails">The <see cref="ValidationProblemDetails"/> instance object containing the validation failures.</param>
        /// <returns>The <see cref="IActionResult"/> object to be executed by the controller action.</returns>
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails);
    }
}