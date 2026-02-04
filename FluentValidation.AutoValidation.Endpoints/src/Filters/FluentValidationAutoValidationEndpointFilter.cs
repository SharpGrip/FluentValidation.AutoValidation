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
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext endpointFilterInvocationContext, EndpointFilterDelegate next)
        {
            var serviceProvider = endpointFilterInvocationContext.HttpContext.RequestServices;

            foreach (var argument in endpointFilterInvocationContext.Arguments)
            {
                if (argument != null && argument.GetType().IsCustomType() && serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
                {
                    var validatorInterceptor = validator as IValidatorInterceptor;
                    var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();

                    IValidationContext validationContext = new ValidationContext<object>(argument);

                    if (validatorInterceptor != null)
                    {
                        validationContext = await validatorInterceptor.BeforeValidation(endpointFilterInvocationContext, validationContext, endpointFilterInvocationContext.HttpContext.RequestAborted) ?? validationContext;
                    }

                    if (globalValidationInterceptor != null)
                    {
                        validationContext = await globalValidationInterceptor.BeforeValidation(endpointFilterInvocationContext, validationContext, endpointFilterInvocationContext.HttpContext.RequestAborted) ?? validationContext;
                    }

                    var validationResult = await validator.ValidateAsync(validationContext, endpointFilterInvocationContext.HttpContext.RequestAborted);

                    if (validatorInterceptor != null)
                    {
                        validationResult = await validatorInterceptor.AfterValidation(endpointFilterInvocationContext, validationContext, validationResult, endpointFilterInvocationContext.HttpContext.RequestAborted) ?? validationResult;
                    }

                    if (globalValidationInterceptor != null)
                    {
                        validationResult = await globalValidationInterceptor.AfterValidation(endpointFilterInvocationContext, validationContext, validationResult, endpointFilterInvocationContext.HttpContext.RequestAborted) ?? validationResult;
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