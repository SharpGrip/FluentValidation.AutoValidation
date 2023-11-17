using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors
{
    public interface IValidationInterceptor
    {
        public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext);
        public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext);
    }
}