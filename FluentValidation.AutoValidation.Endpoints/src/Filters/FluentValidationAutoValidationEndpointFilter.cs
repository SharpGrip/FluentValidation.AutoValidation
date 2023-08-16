using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters
{
    public class FluentValidationAutoValidationEndpointFilter : IEndpointFilter
    {
        private readonly IServiceProvider serviceProvider;

        public FluentValidationAutoValidationEndpointFilter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            for (var i = 0; i < context.Arguments.Count; i++)
            {
                var argument = context.Arguments[i];

                if (argument != null && serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

                    if (!validationResult.IsValid)
                    {
                        var errors = new Dictionary<string, string[]>();

                        foreach (var errorGrouping in validationResult.Errors.GroupBy(error => error.PropertyName))
                        {
                            errors.Add(errorGrouping.Key, errorGrouping.Select(error => error.ErrorMessage).ToArray());
                        }

                        return TypedResults.ValidationProblem(errors);
                    }
                }
            }

            return await next(context);
        }
    }
}