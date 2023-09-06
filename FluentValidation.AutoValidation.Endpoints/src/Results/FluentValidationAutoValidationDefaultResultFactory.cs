using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Results
{
    public class FluentValidationAutoValidationDefaultResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
        {
            return TypedResults.ValidationProblem(validationResult.ToValidationProblemErrors());
        }
    }
}