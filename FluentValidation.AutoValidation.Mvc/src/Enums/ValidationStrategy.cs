namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Enums
{
    public enum ValidationStrategy
    {
        /// <summary>
        /// Enables asynchronous automatic validation on all controllers inheriting from <see cref="T:Microsoft.AspNetCore.Mvc.ControllerBase"/>.
        /// </summary>
        All = 1,

        /// <summary>
        /// Enables asynchronous automatic validation on controllers inheriting from <see cref="T:Microsoft.AspNetCore.Mvc.ControllerBase"/> decorated with a <see cref="T:SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes.FluentValidationAutoValidationAttribute"/> attribute.
        /// </summary>
        Annotations = 2
    }
}