using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Filters;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection serviceCollection)
        {
            var modelStateInvalidFilter = new ModelStateInvalidFilter(new ApiBehaviorOptions {InvalidModelStateResponseFactory = context => new OkResult()}, NullLogger.Instance);

            // Make sure we insert the `FluentValidationAutoValidationActionFilter` before the built-in `ModelStateInvalidFilter` to prevent it short-circuiting the request.
            serviceCollection.Configure<MvcOptions>(options => options.Filters.Add<FluentValidationAutoValidationActionFilter>(modelStateInvalidFilter.Order - 1));

            return serviceCollection;
        }
    }
}