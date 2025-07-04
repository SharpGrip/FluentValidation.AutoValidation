using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration
{
    public class AutoValidationMvcConfiguration
    {
        /// <summary>
        /// Disables the built-in .NET model (data annotations) validation.
        /// </summary>
        public bool DisableBuiltInModelValidation { get; set; }

        /// <summary>
        /// Configures the validation strategy. Validation strategy <see cref="Enums.ValidationStrategy.All"/> enables asynchronous automatic validation on all controllers inheriting from <see cref="ControllerBase"/>.
        /// Validation strategy <see cref="Enums.ValidationStrategy.Annotations"/> enables asynchronous automatic validation on controllers inheriting from <see cref="ControllerBase"/> decorated (class or method) with a <see cref="FluentValidationAutoValidationAttribute"/> attribute.
        /// </summary>
        public ValidationStrategy ValidationStrategy { get; set; } = ValidationStrategy.All;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Body"/> binding sources (typically parameters decorated with the [FromBody] attribute).
        /// </summary>
        /// <see cref="FromBodyAttribute"/>
        public bool EnableBodyBindingSourceAutomaticValidation { get; set; } = true;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Form"/> binding sources (typically parameters decorated with the [FromForm] attribute).
        /// </summary>
        /// <see cref="FromFormAttribute"/>
        public bool EnableFormBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Query"/> binding sources (typically parameters decorated with the [FromQuery] attribute).
        /// </summary>
        /// <see cref="FromQueryAttribute"/>
        public bool EnableQueryBindingSourceAutomaticValidation { get; set; } = true;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Path"/> binding sources (typically parameters decorated with the [FromRoute] attribute).
        /// </summary>
        /// <see cref="FromRouteAttribute"/>
        public bool EnablePathBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Header"/> binding sources (typically parameters decorated with the [FromHeader] attribute).
        /// </summary>
        /// <see cref="FromHeaderAttribute"/>
        public bool EnableHeaderBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from <see cref="BindingSource.Custom"/> binding sources.
        /// </summary>
        public bool EnableCustomBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters not bound from any binding source (typically parameters without a declared or inferred binding source).
        /// </summary>
        public bool EnableNullBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Holds the overridden result factory. This property is meant for infrastructure and should not be used by application code.
        /// </summary>
        public Type? OverriddenResultFactory { get; private set; }

        /// <summary>
        /// Overrides the default result factory with a custom result factory. Custom result factories are required to implement <see cref="IFluentValidationAutoValidationResultFactory"/>.
        /// The default result factory returns the default <see cref="ValidationProblemDetails"/> object wrapped in a <see cref="BadRequestObjectResult"/>>. 
        /// </summary>
        /// <see cref="FluentValidationAutoValidationDefaultResultFactory"/>
        /// <typeparam name="TResultFactory">The custom result factory implement <see cref="IFluentValidationAutoValidationResultFactory"/>.</typeparam>
        public void OverrideDefaultResultFactoryWith<TResultFactory>() where TResultFactory : IFluentValidationAutoValidationResultFactory
        {
            OverriddenResultFactory = typeof(TResultFactory);
        }
    }
}