using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds asynchronous MVC Fluent Validation automatic validation to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="autoValidationMvcConfiguration">The configuration delegate used to configure the FluentValidation AutoValidation MVC validation.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection serviceCollection, Action<AutoValidationMvcConfiguration>? autoValidationMvcConfiguration = null)
        {
            var defaultAutoValidationMvcConfiguration = new AutoValidationMvcConfiguration();

            if (autoValidationMvcConfiguration != null)
            {
                autoValidationMvcConfiguration.Invoke(defaultAutoValidationMvcConfiguration);
                serviceCollection.Configure(autoValidationMvcConfiguration);
            }

            if (defaultAutoValidationMvcConfiguration.DisableDataAnnotationsValidation)
            {
                serviceCollection.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();
            }

            // Create a default instance of the `ModelStateInvalidFilter` to access the non static property `Order` in a static context.
            var modelStateInvalidFilter = new ModelStateInvalidFilter(new ApiBehaviorOptions {InvalidModelStateResponseFactory = context => new OkResult()}, NullLogger.Instance);

            // Make sure we insert the `FluentValidationAutoValidationActionFilter` before the built-in `ModelStateInvalidFilter` to prevent it short-circuiting the request.
            serviceCollection.Configure<MvcOptions>(options => options.Filters.Add<FluentValidationAutoValidationActionFilter>(modelStateInvalidFilter.Order - 1));

            return serviceCollection;
        }
    }
}