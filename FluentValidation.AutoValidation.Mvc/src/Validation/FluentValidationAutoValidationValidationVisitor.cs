using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary? validationState, bool disableBuiltInModelValidation) : ValidationVisitor(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
    {
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel, object? container)
        {
            // If built in model validation is disabled return true for later validation in the action filter.
            return disableBuiltInModelValidation || base.Validate(metadata, key, model, alwaysValidateAtTopLevel, container);
        }
    }
}