namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Enums
{
    public enum ValidationStrategy
    {
        /// <summary>
        /// Enables asynchronous automatic validation on all controllers inheriting from `Microsoft.AspNetCore.Mvc.ControllerBase`.
        /// </summary>
        All = 1,

        /// <summary>
        /// Enables asynchronous automatic validation on controllers inheriting from `Microsoft.AspNetCore.Mvc.ControllerBase` decorated with a `SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes.FluentValidationAutoValidationAttribute` attribute.
        /// </summary>
        Annotation = 2
        
        //Validation strategy `ValidationStrategy.All` enables automatic validation on all instances of `ControllerBase`. Validation strategy `ValidationStrategy.Annotations` only enables automatic validation on all instances of `ControllerBase` with class or method decorations of the `FluentValidationAutoValidation` attribute.
    }
}