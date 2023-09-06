using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds asynchronous Endpoints Fluent Validation automatic validation to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="autoValidationEndpointsConfiguration">The configuration delegate used to configure the FluentValidation AutoValidation Endpoints validation.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection serviceCollection,
            Action<AutoValidationEndpointsConfiguration>? autoValidationEndpointsConfiguration = null)
        {
            var configuration = new AutoValidationEndpointsConfiguration();

            if (autoValidationEndpointsConfiguration != null)
            {
                autoValidationEndpointsConfiguration.Invoke(configuration);
                serviceCollection.Configure(autoValidationEndpointsConfiguration);
            }

            // Add the default result factory.
            serviceCollection.AddScoped<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>();

            // If the custom result factory is not null, replace the default result factory with the overriden result factory.
            if (configuration.OverriddenResultFactory != null)
            {
                serviceCollection.Replace(new ServiceDescriptor(typeof(IFluentValidationAutoValidationResultFactory), configuration.OverriddenResultFactory, ServiceLifetime.Scoped));
            }

            return serviceCollection;
        }
    }
}