using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
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

                if (endpoint != null &&
                    ((autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations &&
                      !endpoint.Metadata.OfType<FluentValidationAutoValidationAttribute>().Any() && !endpoint.Metadata.OfType<AutoValidationAttribute>().Any()) ||
                     endpoint.Metadata.OfType<AutoValidateNeverAttribute>().Any()))
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
                        var bindingSource = parameter.BindingInfo?.BindingSource;

                        var hasAutoValidateAlwaysAttribute = parameterInfo?.HasCustomAttribute<AutoValidateAlwaysAttribute>() ?? false;
                        var hasAutoValidateNeverAttribute = parameterInfo?.HasCustomAttribute<AutoValidateNeverAttribute>() ?? false;

                        if (!hasAutoValidateNeverAttribute && (hasAutoValidateAlwaysAttribute || HasValidBindingSource(bindingSource)))
                        {
                            var validationResult = await FluentValidationHelper.ValidateWithFluentValidationAsync(
                                serviceProvider, subject, actionExecutingContext);
                            if (validationResult != null && !validationResult.IsValid)
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

                    actionExecutingContext.Result = fluentValidationAutoValidationResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails);

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

            return controller is ControllerBase ||
                   controllerType.HasCustomAttribute<ControllerAttribute>() ||
                   controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) ||
                   controllerType.InheritsFromTypeWithNameEndingIn("Controller");
        }

        private bool HasValidBindingSource(BindingSource? bindingSource)
        {
            return (autoValidationMvcConfiguration.EnableBodyBindingSourceAutomaticValidation && bindingSource == BindingSource.Body) ||
                   (autoValidationMvcConfiguration.EnableFormBindingSourceAutomaticValidation && bindingSource == BindingSource.Form) ||
                   (autoValidationMvcConfiguration.EnableQueryBindingSourceAutomaticValidation && bindingSource == BindingSource.Query) ||
                   (autoValidationMvcConfiguration.EnablePathBindingSourceAutomaticValidation && bindingSource == BindingSource.Path) ||
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