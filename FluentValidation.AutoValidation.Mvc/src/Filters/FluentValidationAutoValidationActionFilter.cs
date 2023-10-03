using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
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

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is ControllerBase controllerBase)
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var controllerActionDescriptor = (ControllerActionDescriptor) context.ActionDescriptor;

                if (autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations &&
                    endpoint != null && !endpoint.Metadata.OfType<FluentValidationAutoValidationAttribute>().Any())
                {
                    await next();

                    return;
                }

                foreach (var parameter in controllerActionDescriptor.Parameters)
                {
                    if (context.ActionArguments.TryGetValue(parameter.Name, out var subject))
                    {
                        var parameterType = parameter.ParameterType;
                        var bindingSource = parameter.BindingInfo?.BindingSource;

                        if (subject != null && parameterType.IsCustomType() &&
                            ((autoValidationMvcConfiguration.EnableBodyBindingSourceAutomaticValidation && bindingSource == BindingSource.Body) ||
                             (autoValidationMvcConfiguration.EnableFormBindingSourceAutomaticValidation && bindingSource == BindingSource.Form) ||
                             (autoValidationMvcConfiguration.EnableQueryBindingSourceAutomaticValidation && bindingSource == BindingSource.Query) ||
                             (autoValidationMvcConfiguration.EnableCustomBindingSourceAutomaticValidation && bindingSource == BindingSource.Custom)))
                        {
                            if (serviceProvider.GetValidator(parameterType) is IValidator validator)
                            {
                                var validationResult = await validator.ValidateAsync(new ValidationContext<object>(subject), context.HttpContext.RequestAborted);

                                if (!validationResult.IsValid)
                                {
                                    foreach (var error in validationResult.Errors)
                                    {
                                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                if (autoValidationMvcConfiguration.DisableBuiltInModelValidation)
                {
                    foreach (var modelStateEntry in context.ModelState.Values.Where(modelStateEntry => modelStateEntry.ValidationState == ModelValidationState.Unvalidated))
                    {
                        modelStateEntry.ValidationState = ModelValidationState.Skipped;
                    }
                }

                if (!context.ModelState.IsValid)
                {
                    var validationProblemDetails = controllerBase.ProblemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    context.Result = fluentValidationAutoValidationResultFactory.CreateActionResult(context, validationProblemDetails);

                    return;
                }
            }

            await next();
        }
    }
}