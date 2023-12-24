using System;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class AutoValidateNeverAttribute : Attribute
    {
    }
}