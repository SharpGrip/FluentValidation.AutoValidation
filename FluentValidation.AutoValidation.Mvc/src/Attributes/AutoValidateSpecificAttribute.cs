using System;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AutoValidateSpecificAttribute : Attribute
    {
        public string[] RuleSets { get; }

        public AutoValidateSpecificAttribute(params string[] ruleSets)
        {
            RuleSets = ruleSets;
        }
    }
}
