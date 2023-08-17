using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration
{
    public class AutoValidationMvcConfiguration
    {
        public bool DisableDataAnnotationsValidation { get; set; }
        public ValidationStrategy ValidationStrategy { get; set; } = ValidationStrategy.All;
    }
}