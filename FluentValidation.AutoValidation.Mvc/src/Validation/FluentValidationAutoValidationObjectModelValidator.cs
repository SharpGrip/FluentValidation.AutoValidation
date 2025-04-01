using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
#if !NET
using Microsoft.AspNetCore.Mvc.Internal;
#endif
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationObjectModelValidator : ObjectModelValidator
    {
        private readonly bool disableBuiltInModelValidation;

        public FluentValidationAutoValidationObjectModelValidator(IModelMetadataProvider modelMetadataProvider, IList<IModelValidatorProvider> validatorProviders, bool disableBuiltInModelValidation)
            : base(modelMetadataProvider, validatorProviders)
        {
            this.disableBuiltInModelValidation = disableBuiltInModelValidation;
        }

        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext,
            IModelValidatorProvider validatorProvider,
            ValidatorCache validatorCache,
            IModelMetadataProvider metadataProvider,
            ValidationStateDictionary? validationState)
        {
            return new FluentValidationAutoValidationValidationVisitor(actionContext, validatorProvider, validatorCache, metadataProvider, validationState, disableBuiltInModelValidation);
        }
    }
}