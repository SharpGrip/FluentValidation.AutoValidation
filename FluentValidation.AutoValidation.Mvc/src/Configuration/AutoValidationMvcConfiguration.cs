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
        /// Enables asynchronous automatic validation for parameters bound from the <see cref="BindingSource.Body"/> binding source (typically parameters decorated with the [FormBody] attribute).
        /// </summary>
        public bool EnableBodyBindingSourceAutomaticValidation { get; set; } = true;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from the <see cref="BindingSource.Form"/> binding source (typically parameters decorated with the [FromForm] attribute).
        /// </summary>
        public bool EnableFormBindingSourceAutomaticValidation { get; set; } = false;

        /// <summary>
        /// Enables asynchronous automatic validation for parameters bound from the <see cref="BindingSource.Query"/> binding source (typically parameters decorated with the [FormQuery] attribute).
        /// </summary>
        public bool EnableQueryBindingSourceAutomaticValidation { get; set; } = true;

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