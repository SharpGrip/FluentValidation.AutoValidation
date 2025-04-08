using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds asynchronous MVC Fluent Validation automatic validation to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="autoValidationMvcConfiguration">The configuration delegate used to configure the FluentValidation AutoValidation MVC validation.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection serviceCollection, Action<AutoValidationMvcConfiguration>? autoValidationMvcConfiguration = null)
        {
            var configuration = new AutoValidationMvcConfiguration();

            if (autoValidationMvcConfiguration != null)
            {
                autoValidationMvcConfiguration.Invoke(configuration);
                serviceCollection.Configure(autoValidationMvcConfiguration);
            }

            serviceCollection.AddSingleton<IObjectModelValidator, FluentValidationAutoValidationObjectModelValidator>(serviceProvider =>
                new FluentValidationAutoValidationObjectModelValidator(
                    serviceProvider.GetRequiredService<IModelMetadataProvider>(),
                    serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value.ModelValidatorProviders,
                    configuration.DisableBuiltInModelValidation));

            // Add the default result factory.
            serviceCollection.AddScoped<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>();

            // If the custom result factory is not null, replace the default result factory with the overridden result factory.
            if (configuration.OverriddenResultFactory != null)
            {
                serviceCollection.Replace(new ServiceDescriptor(typeof(IFluentValidationAutoValidationResultFactory), configuration.OverriddenResultFactory, ServiceLifetime.Scoped));
            }

            // Create a default instance of the `ModelStateInvalidFilter` to access the non static property `Order` in a static context.
            var modelStateInvalidFilter = new ModelStateInvalidFilter(new ApiBehaviorOptions {InvalidModelStateResponseFactory = context => new OkResult()}, NullLogger.Instance);

            // Make sure we insert the `FluentValidationAutoValidationActionFilter` before the built-in `ModelStateInvalidFilter` to prevent it short-circuiting the request.
            serviceCollection.Configure<MvcOptions>(options => options.Filters.Add<FluentValidationAutoValidationActionFilter>(modelStateInvalidFilter.Order - 1));

            return serviceCollection;
        }
    }
}