using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationObjectModelValidator : ObjectModelValidator
    {
        private readonly IServiceProvider serviceProvider;
        private readonly bool disableBuiltInModelValidation;

        public FluentValidationAutoValidationObjectModelValidator(
            IServiceProvider serviceProvider,
            IModelMetadataProvider modelMetadataProvider,
            IList<IModelValidatorProvider> validatorProviders, bool disableBuiltInModelValidation)
            : base(modelMetadataProvider, validatorProviders)
        {
            this.serviceProvider = serviceProvider;
            this.disableBuiltInModelValidation = disableBuiltInModelValidation;
        }

        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext,
            IModelValidatorProvider validatorProvider,
            ValidatorCache validatorCache,
            IModelMetadataProvider metadataProvider,
            ValidationStateDictionary? validationState)
        {
            return new FluentValidationAutoValidationValidationVisitor(
                serviceProvider,
                actionContext,
                validatorProvider,
                validatorCache,
                metadataProvider,
                validationState,
                disableBuiltInModelValidation);
        }
    }
}