using System;
using Microsoft.AspNetCore.Http.HttpResults;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;

namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Configuration
{
    public class AutoValidationEndpointsConfiguration
    {
        /// <summary>
        /// Holds the overridden result factory. This property is meant for infrastructure and should not be used by application code.
        /// </summary>
        public Type? OverriddenResultFactory { get; private set; }

        /// <summary>
        /// Overrides the default result factory with a custom result factory. Custom result factories are required to implement <see cref="IFluentValidationAutoValidationResultFactory"/>.
        /// The default result factory returns the validation errors wrapped in a <see cref="ValidationProblem"/> object. 
        /// </summary>
        /// <see cref="FluentValidationAutoValidationDefaultResultFactory"/>
        /// <typeparam name="TResultFactory">The custom result factory implementing <see cref="IFluentValidationAutoValidationResultFactory"/>.</typeparam>
        public void OverrideDefaultResultFactoryWith<TResultFactory>() where TResultFactory : IFluentValidationAutoValidationResultFactory
        {
            OverriddenResultFactory = typeof(TResultFactory);
        }
    }
}