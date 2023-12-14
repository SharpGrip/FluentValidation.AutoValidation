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
            foreach (var argument in endpointFilterInvocationContext.Arguments)
            {
                if (argument != null && argument.GetType().IsCustomType() && serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    var validatorInterceptor = validator as IValidatorInterceptor;
                    var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();

                    IValidationContext validationContext = new ValidationContext<object>(argument);

                    if (validatorInterceptor != null)
                    {
                        validationContext = validatorInterceptor.BeforeValidation(endpointFilterInvocationContext, validationContext) ?? validationContext;
                    }

                    if (globalValidationInterceptor != null)
                    {
                        validationContext = globalValidationInterceptor.BeforeValidation(endpointFilterInvocationContext, validationContext) ?? validationContext;
                    }

                    var validationResult = await validator.ValidateAsync(validationContext, endpointFilterInvocationContext.HttpContext.RequestAborted);

                    if (validatorInterceptor != null)
                    {
                        validationResult = validatorInterceptor.AfterValidation(endpointFilterInvocationContext, validationContext) ?? validationResult;
                    }

                    if (globalValidationInterceptor != null)
                    {
                        validationResult = globalValidationInterceptor.AfterValidation(endpointFilterInvocationContext, validationContext) ?? validationResult;
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