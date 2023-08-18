using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration
{
    public class AutoValidationMvcConfiguration
    {
        /// <summary>
        /// Disables the built-in model validation. 
        /// </summary>
        public bool DisableBuiltInModelValidation { get; set; }

        /// <summary>
        /// Configures the validation strategy. Validation strategy <see cref="T:SharpGrip.FluentValidation.AutoValidation.Mvc.Enums.ValidationStrategy"/> enables asynchronous automatic validation on all controllers inheriting from <see cref="T:Microsoft.AspNetCore.Mvc.ControllerBase"/>.
        /// Validation strategy <see cref="SharpGrip.FluentValidation.AutoValidation.Mvc.Enums.ValidationStrategy.Annotations"/> enables asynchronous automatic validation on controllers inheriting from <see cref="T:Microsoft.AspNetCore.Mvc.ControllerBase"/> decorated (class or method) with a <see cref="T:SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes.FluentValidationAutoValidationAttribute"/> attribute.
        /// </summary>
        public ValidationStrategy ValidationStrategy { get; set; } = ValidationStrategy.All;
    }
}