using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Filters
{
    public class FluentValidationAutoValidationActionFilter(IFluentValidationAutoValidationResultFactory fluentValidationAutoValidationResultFactory, IOptions<AutoValidationMvcConfiguration> autoValidationMvcConfiguration, ILogger<FluentValidationAutoValidationActionFilter> logger) : IAsyncActionFilter
    {
        private readonly AutoValidationMvcConfiguration autoValidationMvcConfiguration = autoValidationMvcConfiguration.Value;

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor) actionExecutingContext.ActionDescriptor;

            logger.LogDebug("Starting validation for action '{Action}' on controller '{Controller}'.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);

            if (IsValidController(actionExecutingContext.Controller))
            {
                var endpoint = actionExecutingContext.HttpContext.GetEndpoint();
                var serviceProvider = actionExecutingContext.HttpContext.RequestServices;

                if (endpoint != null && ((autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations && !endpoint.Metadata.OfType<AutoValidationAttribute>().Any()) || endpoint.Metadata.OfType<AutoValidateNeverAttribute>().Any()))
                {
                    logger.LogDebug("Skipping validation for action '{Action}' on controller '{Controller}' due to validation strategy or AutoValidateNeverAttribute.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);

                    HandleUnvalidatedEntries(actionExecutingContext);

                    await next();

                    return;
                }

                var validationResults = new Dictionary<IValidationContext, ValidationResult>();

                foreach (var parameter in controllerActionDescriptor.Parameters)
                {
                    if (actionExecutingContext.ActionArguments.TryGetValue(parameter.Name, out var subject))
                    {
                        var parameterInfo = (parameter as ControllerParameterDescriptor)?.ParameterInfo;
                        var parameterType = subject?.GetType();
                        var bindingSource = parameter.BindingInfo?.BindingSource;

                        var hasAutoValidateAlwaysAttribute = parameterInfo?.HasCustomAttribute<AutoValidateAlwaysAttribute>() ?? false;
                        var hasAutoValidateNeverAttribute = parameterInfo?.HasCustomAttribute<AutoValidateNeverAttribute>() ?? false;

                        if (subject != null && parameterType != null && parameterType.IsCustomType() && !hasAutoValidateNeverAttribute && (hasAutoValidateAlwaysAttribute || HasValidBindingSource(bindingSource)) && serviceProvider.GetValidator(parameterType) is IValidator validator)
                        {
                            logger.LogDebug("Validating parameter '{Parameter}' of type '{Type}' for action '{Action}' on controller '{Controller}'.", parameter.Name, parameterType.Name, controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);

                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var validatorInterceptor = validator as IValidatorInterceptor;
                            var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();

                            IValidationContext validationContext = new ValidationContext<object>(subject);

                            if (validatorInterceptor != null)
                            {
                                logger.LogDebug("Invoking validator interceptor BeforeValidation for parameter '{Parameter}'.", parameter.Name);
                                validationContext = await validatorInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
                            }

                            if (globalValidationInterceptor != null)
                            {
                                logger.LogDebug("Invoking global validation interceptor BeforeValidation for parameter '{Parameter}'.", parameter.Name);
                                validationContext = await globalValidationInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
                            }

                            var validationResult = await validator.ValidateAsync(validationContext, actionExecutingContext.HttpContext.RequestAborted);
                            validationResults.Add(validationContext, validationResult);

                            if (validatorInterceptor != null)
                            {
                                logger.LogDebug("Invoking validator interceptor AfterValidation for parameter '{Parameter}'.", parameter.Name);
                                validationResult = await validatorInterceptor.AfterValidation(actionExecutingContext, validationContext, validationResult) ?? validationResult;
                            }

                            if (globalValidationInterceptor != null)
                            {
                                logger.LogDebug("Invoking global validation interceptor AfterValidation for parameter '{Parameter}'.", parameter.Name);
                                validationResult = await globalValidationInterceptor.AfterValidation(actionExecutingContext, validationContext, validationResult) ?? validationResult;
                            }

                            if (!validationResult.IsValid)
                            {
                                logger.LogDebug("Validation result not valid for parameter '{Parameter}' of type '{Type}' for action '{Action}' on controller '{Controller}': {ErrorCount} validation error(s) found.", parameter.Name, parameterType.Name, controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName, validationResult.Errors.Count);

                                foreach (var error in validationResult.Errors)
                                {
                                    logger.LogTrace("Adding validation error '{ErrorMessage}' for '{ParameterName}' to ModelState.", error.ErrorMessage, parameter.Name);

                                    actionExecutingContext.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                                }
                            }
                            else
                            {
                                logger.LogDebug("Validation result valid for parameter '{Parameter}' of type '{Type}' for action '{Action}' on controller '{Controller}'.", parameter.Name, parameterType.Name, controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);
                            }
                        }
                    }
                }

                HandleUnvalidatedEntries(actionExecutingContext);

                if (!actionExecutingContext.ModelState.IsValid)
                {
                    logger.LogDebug("ModelState is not valid for action '{Action}' on controller '{Controller}'. Creating validation problem details.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);

                    var problemDetailsFactory = serviceProvider.GetRequiredService<ProblemDetailsFactory>();
                    var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(actionExecutingContext.HttpContext, actionExecutingContext.ModelState);

                    logger.LogTrace("Creating action result for action '{Action}' on controller '{Controller}'.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);

                    actionExecutingContext.Result = await fluentValidationAutoValidationResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails, validationResults);

                    if (actionExecutingContext.Result != null)
                    {
                        logger.LogTrace("Action result created for action '{Action}' on controller '{Controller}'.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);
                    }

                    logger.LogTrace("No action result created for action '{Action}' on controller '{Controller}'.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);
                }

                logger.LogDebug("ModelState is valid for action '{Action}' on controller '{Controller}'. Proceeding with action execution.", controllerActionDescriptor.ActionName, controllerActionDescriptor.ControllerName);
            }

            await next();
        }

        private bool IsValidController(object controller)
        {
            var controllerType = controller.GetType();

            if (controllerType.HasCustomAttribute<NonControllerAttribute>())
            {
                logger.LogDebug("Controller '{Controller}' is marked with NonControllerAttribute. Skipping validation.", controllerType.Name);

                return false;
            }

            return controller is ControllerBase || controllerType.HasCustomAttribute<ControllerAttribute>() || controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) || controllerType.InheritsFromTypeWithNameEndingIn("Controller");
        }

        private bool HasValidBindingSource(BindingSource? bindingSource)
        {
            return (autoValidationMvcConfiguration.EnableBodyBindingSourceAutomaticValidation && bindingSource == BindingSource.Body) ||
                   (autoValidationMvcConfiguration.EnableFormBindingSourceAutomaticValidation && bindingSource == BindingSource.Form) ||
                   (autoValidationMvcConfiguration.EnableQueryBindingSourceAutomaticValidation && bindingSource == BindingSource.Query) ||
                   (autoValidationMvcConfiguration.EnablePathBindingSourceAutomaticValidation && bindingSource == BindingSource.Path) ||
                   (autoValidationMvcConfiguration.EnableHeaderBindingSourceAutomaticValidation && bindingSource == BindingSource.Header) ||
                   (autoValidationMvcConfiguration.EnableCustomBindingSourceAutomaticValidation && bindingSource == BindingSource.Custom) ||
                   (autoValidationMvcConfiguration.EnableNullBindingSourceAutomaticValidation && bindingSource == null);
        }

        private void HandleUnvalidatedEntries(ActionExecutingContext context)
        {
            if (autoValidationMvcConfiguration.DisableBuiltInModelValidation)
            {
                logger.LogDebug("Skipping validation of unvalidated entries due to DisableBuiltInModelValidation being set to true.");

                foreach (var modelStateEntry in context.ModelState.Values.Where(modelStateEntry => modelStateEntry.ValidationState == ModelValidationState.Unvalidated))
                {
                    modelStateEntry.ValidationState = ModelValidationState.Skipped;
                }
            }
        }
    }
}