using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Filters
{
    public class FluentValidationAutoValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider serviceProvider;

        public FluentValidationAutoValidationActionFilter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is ControllerBase controllerBase)
            {
                foreach (var parameter in context.ActionDescriptor.Parameters)
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