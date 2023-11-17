using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
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

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext endpointFilterInvocationContext, EndpointFilterDelegate next)
        {
            for (var i = 0; i < endpointFilterInvocationContext.Arguments.Count; i++)
            {
                var argument = endpointFilterInvocationContext.Arguments[i];

                if (argument != null && argument.GetType().IsCustomType() && serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
                {
                    var validationInterceptor = serviceProvider.GetService<IValidationInterceptor>();
                    IValidationContext validationContext = new ValidationContext<object>(argument);

                    if (validationInterceptor != null)
                    {
                        validationContext = validationInterceptor.BeforeValidation(endpointFilterInvocationContext, validationContext) ?? validationContext;
                    }

                    var validationResult = await validator.ValidateAsync(validationContext, endpointFilterInvocationContext.HttpContext.RequestAborted);

                    if (validationInterceptor != null)
                    {
                        validationResult = validationInterceptor.AfterValidation(endpointFilterInvocationContext, validationContext) ?? validationResult;
                    }

                    if (!validationResult.IsValid)
                    {
                        var fluentValidationAutoValidationResultFactory = serviceProvider.GetService<IFluentValidationAutoValidationResultFactory>();

                        if (fluentValidationAutoValidationResultFactory != null)
                        {
                            return fluentValidationAutoValidationResultFactory.CreateResult(endpointFilterInvocationContext, validationResult);
                        }

                        return new FluentValidationAutoValidationDefaultResultFactory().CreateResult(endpointFilterInvocationContext, validationResult);
                    }
                }
            }

            return await next(endpointFilterInvocationContext);
        }
    }
}