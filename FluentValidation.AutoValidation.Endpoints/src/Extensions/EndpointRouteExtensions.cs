using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions
{
    public static class EndpointRouteExtensions
    {
        /// <summary>
        /// Adds asynchronous minimal API automatic validation to the specified <see cref="T:Microsoft.AspNetCore.Builder.RouteHandlerBuilder" />.
        /// </summary>
        /// <param name="routeHandlerBuilder">The route handler builder.</param>
        /// <returns>The route handler builder.</returns>
        public static RouteHandlerBuilder AddFluentValidationAutoValidation(this RouteHandlerBuilder routeHandlerBuilder)
        {
            routeHandlerBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeHandlerBuilder;
        }

        /// <summary>
        /// Adds asynchronous minimal API Fluent Validation automatic validation to the specified <see cref="T:Microsoft.AspNetCore.Routing.RouteGroupBuilder" />.
        /// </summary>
        /// <param name="routeGroupBuilder">The route group builder.</param>
        /// <returns>The route group builder.</returns>
        public static RouteGroupBuilder AddFluentValidationAutoValidation(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeGroupBuilder;
        }
    }
}