using System;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes
{
    [Obsolete("Attribute is obsolete and will be removed in v2. Use the [AutoValidation] attribute instead.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FluentValidationAutoValidationAttribute : Attribute
    {
    }
}