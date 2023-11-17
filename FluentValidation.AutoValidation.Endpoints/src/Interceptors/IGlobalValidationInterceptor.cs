using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors
{
    /// <summary>
    /// Allows global intercepting and altering of the validation process by implementing this interface on a custom class and registering it with the service collection.
    ///
    /// The interceptor methods of instances of this interface will get called on each validation attempt.
    /// </summary>
    public interface IGlobalValidationInterceptor
    {
        public IValidationContext? BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
        public ValidationResult? AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
    }
}