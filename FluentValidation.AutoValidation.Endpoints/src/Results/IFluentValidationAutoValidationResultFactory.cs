using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Results
{
    public interface IFluentValidationAutoValidationResultFactory
    {
        /// <summary>
        /// Creates an <see cref="IResult"/> object to be executed by the endpoint.
        /// </summary>
        /// <param name="context">The <see cref="EndpointFilterInvocationContext"/> associated with the current request/response.</param>
        /// <param name="validationResult">The <see cref="ValidationResult"/> object containing the validation failures.</param>
        /// <returns>The <see cref="IResult"/> object to be executed by the endpoint.</returns>
        public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult);
    }
}