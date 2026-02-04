using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationObjectModelValidator(IModelMetadataProvider modelMetadataProvider, IList<IModelValidatorProvider> validatorProviders, bool disableBuiltInModelValidation) : ObjectModelValidator(modelMetadataProvider, validatorProviders)
    {
        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary? validationState)
        {
            return new FluentValidationAutoValidationValidationVisitor(actionContext, validatorProvider, validatorCache, metadataProvider, validationState, disableBuiltInModelValidation);
        }
    }
}