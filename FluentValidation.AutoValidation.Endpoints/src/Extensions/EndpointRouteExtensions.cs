using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions
{
    public static class EndpointRouteExtensions
    {
        /// <summary>
        /// Adds asynchronous minimal API automatic validation to the specified <see cref="RouteHandlerBuilder" />.
        /// </summary>
        /// <param name="routeHandlerBuilder">The route handler builder.</param>
        /// <returns>The route handler builder.</returns>
        /// <remarks>
        /// For AOT validation to work, the validator itself must be marked [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(YourValidator!)] to ensure it's preserved.<br/>
        /// Otherwise, it will not be found at runtime and the validation will NOT happen.<br/>
        /// </remarks>
        [RequiresUnreferencedCode("Requires unreferenced code to locate and access IValidator<> types at runtime, it only works for types that are known (and preserved) at compile time. See remarks for more details.")]
        public static RouteHandlerBuilder AddFluentValidationAutoValidation(this RouteHandlerBuilder routeHandlerBuilder)
        {
            routeHandlerBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeHandlerBuilder;
        }

        /// <summary>
        /// Adds asynchronous minimal API Fluent Validation automatic validation to the specified <see cref="RouteGroupBuilder" />.
        /// </summary>
        /// <param name="routeGroupBuilder">The route group builder.</param>
        /// <returns>The route group builder.</returns>
        /// <remarks>
        /// For AOT validation to work, the validator itself must be marked as DynamicDependency to ensure it's preserved:<code>[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(YourValidator!)]</code><br/>
        /// <b>Otherwise, it will not be found at runtime and the validation will NOT happen.</b><br/>
        /// </remarks>
        [RequiresUnreferencedCode("Requires unreferenced code to locate and access IValidator<> types at runtime, it only works for types that are known (and preserved) at compile time. See remarks for more details.")]
        public static RouteGroupBuilder AddFluentValidationAutoValidation(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeGroupBuilder;
        }
    }
}