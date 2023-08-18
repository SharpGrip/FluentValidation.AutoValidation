using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationValidationVisitor : ValidationVisitor
    {
        private readonly bool disableBuiltInModelValidation;

        private readonly List<Type> systemTypes = Assembly.GetExecutingAssembly().GetType().Module.Assembly.GetExportedTypes().ToList();

        public FluentValidationAutoValidationValidationVisitor(ActionContext actionContext,
            IModelValidatorProvider validatorProvider,
            ValidatorCache validatorCache,
            IModelMetadataProvider metadataProvider,
            ValidationStateDictionary? validationState,
            bool disableBuiltInModelValidation)
            : base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
        {
            this.disableBuiltInModelValidation = disableBuiltInModelValidation;
        }

        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel)
        {
            // For non-system class types return true for later validation in the action filter. For all other (system) types return the base validation result.
            if (IsBuiltInValidationDisabledAndTypeIsClassAndNotSystemType(model))
            {
                return true;
            }

            return base.Validate(metadata, key, model, alwaysValidateAtTopLevel);
        }

#if !NETCOREAPP3_1
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel, object? container)
        {
            // For non-system class types return true for later validation in the action filter. For all other (system) types return the base validation result.
            if (IsBuiltInValidationDisabledAndTypeIsClassAndNotSystemType(model))
            {
                return true;
            }

            return base.Validate(metadata, key, model, alwaysValidateAtTopLevel, container);
        }
#endif

        private bool IsBuiltInValidationDisabledAndTypeIsClassAndNotSystemType(object? model)
        {
            var modelType = model?.GetType();

            return disableBuiltInModelValidation && modelType != null && modelType.IsClass && !systemTypes.Contains(modelType);
        }
    }
}