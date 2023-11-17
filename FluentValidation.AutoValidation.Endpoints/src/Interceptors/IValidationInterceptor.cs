using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors
{
    public interface IValidationInterceptor
    {
        public IValidationContext? BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
        public ValidationResult? AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
    }
}