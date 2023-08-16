using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions
{
    public static class EndpointRouteExtensions
    {
        public static RouteHandlerBuilder AddFluentValidationAutoValidation(this RouteHandlerBuilder routeHandlerBuilder)
        {
            routeHandlerBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeHandlerBuilder;
        }

        public static RouteGroupBuilder AddFluentValidationAutoValidation(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

            return routeGroupBuilder;
        }
    }
}