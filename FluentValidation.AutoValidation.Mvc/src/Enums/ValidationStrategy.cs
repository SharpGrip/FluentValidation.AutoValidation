using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Enums
{
    public enum ValidationStrategy
    {
        /// <summary>
        /// Enables asynchronous automatic validation on all controllers inheriting from <see cref="ControllerBase"/>.
        /// </summary>
        All = 1,

        /// <summary>
        /// Enables asynchronous automatic validation on controllers inheriting from <see cref="ControllerBase"/> decorated with a <see cref="AutoValidationAttribute"/> attribute.
        /// </summary>
        Annotations = 2
    }
}