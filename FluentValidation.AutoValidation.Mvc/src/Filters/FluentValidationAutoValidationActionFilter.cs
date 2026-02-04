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
        private readonly IFluentValidationAutoValidationResultFactory fluentValidationAutoValidationResultFactory;
        private readonly AutoValidationMvcConfiguration autoValidationMvcConfiguration;

        public FluentValidationAutoValidationActionFilter(IFluentValidationAutoValidationResultFactory fluentValidationAutoValidationResultFactory, IOptions<AutoValidationMvcConfiguration> autoValidationMvcConfiguration)
        {
            this.fluentValidationAutoValidationResultFactory = fluentValidationAutoValidationResultFactory;
            this.autoValidationMvcConfiguration = autoValidationMvcConfiguration.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            if (IsValidController(actionExecutingContext.Controller))
            {
                var endpoint = actionExecutingContext.HttpContext.GetEndpoint();
                var controllerActionDescriptor = (ControllerActionDescriptor) actionExecutingContext.ActionDescriptor;
                var serviceProvider = actionExecutingContext.HttpContext.RequestServices;

                if (endpoint != null && ((autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations && !endpoint.Metadata.OfType<AutoValidationAttribute>().Any()) || endpoint.Metadata.OfType<AutoValidateNeverAttribute>().Any()))
                {
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
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var validatorInterceptor = validator as IValidatorInterceptor;
                            var globalValidationInterceptor = serviceProvider.GetService<IGlobalValidationInterceptor>();

                            IValidationContext validationContext = new ValidationContext<object>(subject);

                            if (validatorInterceptor != null)
                            {
                                validationContext = await validatorInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
                            }

                            if (globalValidationInterceptor != null)
                            {
                                validationContext = await globalValidationInterceptor.BeforeValidation(actionExecutingContext, validationContext) ?? validationContext;
                            }

                            var validationResult = await validator.ValidateAsync(validationContext, actionExecutingContext.HttpContext.RequestAborted);
                            validationResults.Add(validationContext, validationResult);

                            if (validatorInterceptor != null)
                            {
                                validationResult = await validatorInterceptor.AfterValidation(actionExecutingContext, validationContext, validationResult) ?? validationResult;
                            }

                            if (globalValidationInterceptor != null)
                            {
                                validationResult = await globalValidationInterceptor.AfterValidation(actionExecutingContext, validationContext, validationResult) ?? validationResult;
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

                HandleUnvalidatedEntries(actionExecutingContext);

                if (!actionExecutingContext.ModelState.IsValid)
                {
                    var problemDetailsFactory = serviceProvider.GetRequiredService<ProblemDetailsFactory>();
                    var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(actionExecutingContext.HttpContext, actionExecutingContext.ModelState);

                    actionExecutingContext.Result = await fluentValidationAutoValidationResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails, validationResults);

                    return;
                }
            }

            await next();
        }

        private bool IsValidController(object controller)
        {
            var controllerType = controller.GetType();

            if (controllerType.HasCustomAttribute<NonControllerAttribute>())
            {
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
                foreach (var modelStateEntry in context.ModelState.Values.Where(modelStateEntry => modelStateEntry.ValidationState == ModelValidationState.Unvalidated))
                {
                    modelStateEntry.ValidationState = ModelValidationState.Skipped;
                }
            }
        }
    }
}