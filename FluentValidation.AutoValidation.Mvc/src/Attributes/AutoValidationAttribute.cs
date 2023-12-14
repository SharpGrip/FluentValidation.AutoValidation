using System;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutoValidationAttribute : Attribute
    {
    }
}