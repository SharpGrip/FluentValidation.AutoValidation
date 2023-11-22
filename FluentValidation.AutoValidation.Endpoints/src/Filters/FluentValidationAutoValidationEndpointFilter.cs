using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters
{
    public class FluentValidationAutoValidationEndpointFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            for (var i = 0; i < context.Arguments.Count; i++)
            {
                var argument = context.Arguments[i];

                if (argument != null && argument.GetType().IsCustomType() && context.HttpContext.RequestServices.GetValidator(argument.GetType()) is IValidator validator)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

                    if (!validationResult.IsValid)
                    {
                        var fluentValidationAutoValidationResultFactory = context.HttpContext.RequestServices.GetService<IFluentValidationAutoValidationResultFactory>();

                        if (fluentValidationAutoValidationResultFactory != null)
                        {
                            return fluentValidationAutoValidationResultFactory.CreateResult(context, validationResult);
                        }

                        return new FluentValidationAutoValidationDefaultResultFactory().CreateResult(context, validationResult);
                    }
                }
            }

            return await next(context);
        }
    }
}