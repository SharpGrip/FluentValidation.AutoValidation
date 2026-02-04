using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
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
        /// <param name="validationResults">The dictionary of <see cref="ValidationResult"/> instances keyed by the models containing the validation results from all validators that were executed.</param>
        /// <returns>The <see cref="IActionResult"/> object to be executed by the controller action or null to prevent short-circuiting the action.</returns>
        public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults);
    }
}