using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Filters
{
    public class FluentValidationAutoValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly AutoValidationMvcConfiguration autoValidationMvcConfiguration;

        public FluentValidationAutoValidationActionFilter(IServiceProvider serviceProvider, IOptions<AutoValidationMvcConfiguration> autoValidationMvcConfiguration)
        {
            this.serviceProvider = serviceProvider;
            this.autoValidationMvcConfiguration = autoValidationMvcConfiguration.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is ControllerBase controllerBase)
            {
                var actionDescriptor = context.ActionDescriptor;

                // @todo figure out a better way to retrieve the attribute since using the `context.ActionDescriptor.EndpointMetadata` is not recommended for application code
                if (autoValidationMvcConfiguration.ValidationStrategy == ValidationStrategy.Annotations && !actionDescriptor.EndpointMetadata.OfType<FluentValidationAutoValidationAttribute>().Any())
                {
                    await next();

                    return;
                }

                foreach (var parameter in actionDescriptor.Parameters)
                {
                    var subject = context.ActionArguments[parameter.Name];
                    var parameterType = parameter.ParameterType;

                    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                    var bindingSource = parameter.BindingInfo?.BindingSource;

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (subject != null && (bindingSource == BindingSource.Body || (bindingSource == BindingSource.Query && parameterType.IsClass)))
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

                if (!context.ModelState.IsValid)
                {
                    var validationProblem = controllerBase.ProblemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    context.Result = new BadRequestObjectResult(validationProblem);

                    return;
                }
            }

            await next();
        }
    }
}