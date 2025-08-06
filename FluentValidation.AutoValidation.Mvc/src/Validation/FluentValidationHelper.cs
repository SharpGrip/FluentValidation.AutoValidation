using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public static class FluentValidationHelper
    {
        public static async Task<ValidationResult?> ValidateWithFluentValidationAsync(
            IServiceProvider serviceProvider,
            object? model,
            ActionExecutingContext actionExecutingContext)
        {
            if (model == null)
            {
                return null;
            }

            var modelType = model.GetType();
            if (modelType == null)
            {
                return null;
            }

            if (!modelType.IsCustomType())
            {
                return null;
            }

            var validator = serviceProvider.GetValidator(modelType) as IValidator;
            if (validator == null)
            {
                return null;
            }

            IValidationContext validationContext = new ValidationContext<object>(model);

            var validatorInterceptor = validator as IValidatorInterceptor;
            if (validatorInterceptor != null)
            {
                validationContext = validatorInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
            }

            var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();
            if (globalValidationInterceptor != null)
            {
                validationContext = globalValidationInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
            }

            var validationResult = await validator.ValidateAsync(validationContext, actionExecutingContext.HttpContext.RequestAborted);

            if (validatorInterceptor != null)
            {
                validationResult = validatorInterceptor.AfterValidation(actionExecutingContext, validationContext) ?? validationResult;
            }

            if (globalValidationInterceptor != null)
            {
                validationResult = globalValidationInterceptor.AfterValidation(actionExecutingContext, validationContext) ?? validationResult;
            }

            return validationResult;
        }
    }
}
