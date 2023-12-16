using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Filters
{
    public class FluentValidationAutoValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IFluentValidationAutoValidationResultFactory fluentValidationAutoValidationResultFactory;
        private readonly AutoValidationMvcConfiguration autoValidationMvcConfiguration;

        public FluentValidationAutoValidationActionFilter(IServiceProvider serviceProvider,
            IFluentValidationAutoValidationResultFactory fluentValidationAutoValidationResultFactory,
            IOptions<AutoValidationMvcConfiguration> autoValidationMvcConfiguration)
        {
            this.serviceProvider = serviceProvider;
            this.fluentValidationAutoValidationResultFactory = fluentValidationAutoValidationResultFactory;
            this.autoValidationMvcConfiguration = autoValidationMvcConfiguration.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            if (actionExecutingContext.Controller is ControllerBase controllerBase)
            {
                var endpoint = actionExecutingContext.HttpContext.GetEndpoint();
                var controllerActionDescriptor = (ControllerActionDescriptor) actionExecutingContext.ActionDescriptor;

                if (endpoint != null &&
                    ((autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations &&
                      !endpoint.Metadata.OfType<FluentValidationAutoValidationAttribute>().Any() && !endpoint.Metadata.OfType<AutoValidationAttribute>().Any()) ||
                     endpoint.Metadata.OfType<AutoValidateNever>().Any()))
                {
                    HandleUnvalidatedEntries(actionExecutingContext);

                    await next();

                    return;
                }

                foreach (var parameter in controllerActionDescriptor.Parameters)
                {
                    if (actionExecutingContext.ActionArguments.TryGetValue(parameter.Name, out var subject))
                    {
                        var parameterInfo = (parameter as ControllerParameterDescriptor)?.ParameterInfo;
                        var parameterType = parameter.ParameterType;
                        var bindingSource = parameter.BindingInfo?.BindingSource;

                        var hasAutoValidateAlwaysAttribute = parameterInfo?.HasCustomAttribute<AutoValidateAlways>() ?? false;
                        var hasAutoValidateNeverAttribute = parameterInfo?.HasCustomAttribute<AutoValidateNever>() ?? false;

                        if (subject != null && parameterType.IsCustomType() && !hasAutoValidateNeverAttribute && (hasAutoValidateAlwaysAttribute || HasValidBindingSource(bindingSource)))
                        {
                            if (serviceProvider.GetValidator(parameterType) is IValidator validator)
                            {
                                // ReSharper disable once SuspiciousTypeConversion.Global
                                var validatorInterceptor = validator as IValidatorInterceptor;
                                var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();

                                IValidationContext validationContext = new ValidationContext<object>(subject);

                                if (validatorInterceptor != null)
                                {
                                    validationContext = validatorInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
                                }

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

                                if (!validationResult.IsValid)
                                {
                                    foreach (var error in validationResult.Errors)
                                    {
                                        actionExecutingContext.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                HandleUnvalidatedEntries(actionExecutingContext);

                if (!actionExecutingContext.ModelState.IsValid)
                {
                    var validationProblemDetails = controllerBase.ProblemDetailsFactory.CreateValidationProblemDetails(actionExecutingContext.HttpContext, actionExecutingContext.ModelState);

                    actionExecutingContext.Result = fluentValidationAutoValidationResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails);

                    return;
                }
            }

            await next();
        }

        private bool HasValidBindingSource(BindingSource? bindingSource)
        {
            return (autoValidationMvcConfiguration.EnableBodyBindingSourceAutomaticValidation && bindingSource == BindingSource.Body) ||
                   (autoValidationMvcConfiguration.EnableFormBindingSourceAutomaticValidation && bindingSource == BindingSource.Form) ||
                   (autoValidationMvcConfiguration.EnableQueryBindingSourceAutomaticValidation && bindingSource == BindingSource.Query) ||
                   (autoValidationMvcConfiguration.EnablePathBindingSourceAutomaticValidation && bindingSource == BindingSource.Path) ||
                   (autoValidationMvcConfiguration.EnableCustomBindingSourceAutomaticValidation && bindingSource == BindingSource.Custom);
        }

        private void HandleUnvalidatedEntries(ActionExecutingContext context)
        {
            if (autoValidationMvcConfiguration.DisableBuiltInModelValidation)
            {
                foreach (var modelStateEntry in context.ModelState.Values.Where(modelStateEntry => modelStateEntry.ValidationState == ModelValidationState.Unvalidated))
                {
                    modelStateEntry.ValidationState = ModelValidationState.Skipped;
                }
            }
        }
    }
}