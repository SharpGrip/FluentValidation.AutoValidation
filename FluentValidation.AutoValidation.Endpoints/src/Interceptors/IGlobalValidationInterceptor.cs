using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// Executes custom logic before the validation process. Allows intercepting and altering the validation context
        /// prior to the execution of the validation rules.
        /// </summary>
        /// <param name="endpointFilterInvocationContext">The context of the currently executing endpoint filter, providing access to details about the HTTP request and endpoint.</param>
        /// <param name="validationContext">The validation context containing information about the object being validated.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A transformed or new <see cref="IValidationContext"/> instance to be used in the validation process,
        /// or null if no changes need to be applied.
        /// </returns>
        public Task<IValidationContext?> BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes custom logic after the validation process. Allows intercepting and altering the validation result
        /// or performing additional operations after the validation has been completed.
        /// </summary>
        /// <param name="endpointFilterInvocationContext">The context of the currently executing endpoint filter, providing access to details about the HTTP request and endpoint.</param>
        /// <param name="validationContext">The validation context containing information about the object that was validated.</param>
        /// <param name="validationResult">The result of the validation process, including validation errors if any exist.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A modified or new <see cref="ValidationResult"/> instance, or null if no changes are required to the validation result.
        /// </returns>
        public Task<ValidationResult?> AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, ValidationResult validationResult, CancellationToken cancellationToken = default);
    }
}