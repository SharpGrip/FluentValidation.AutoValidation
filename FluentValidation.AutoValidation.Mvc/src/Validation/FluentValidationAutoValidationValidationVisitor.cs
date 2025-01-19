using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationValidationVisitor : ValidationVisitor
    {
        private readonly IServiceProvider serviceProvider;
        private readonly bool disableBuiltInModelValidation;

        public FluentValidationAutoValidationValidationVisitor(
            IServiceProvider serviceProvider,
            ActionContext actionContext,
            IModelValidatorProvider validatorProvider,
            ValidatorCache validatorCache,
            IModelMetadataProvider metadataProvider,
            ValidationStateDictionary? validationState,
            bool disableBuiltInModelValidation)
            : base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
        {
            this.serviceProvider = serviceProvider;
            this.disableBuiltInModelValidation = disableBuiltInModelValidation;
        }

        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel)
        {
            // If built in model validation is disabled return true for later validation in the action filter.
            bool isBaseValid = disableBuiltInModelValidation || base.Validate(metadata, key, model, alwaysValidateAtTopLevel);
            return Validate(isBaseValid, key, model);
        }

#if !NETCOREAPP3_1
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel, object? container)
        {
            // If built in model validation is disabled return true for later validation in the action filter.
            bool isBaseValid = disableBuiltInModelValidation || base.Validate(metadata, key, model, alwaysValidateAtTopLevel, container);
            return Validate(isBaseValid, key, model);
        }
#endif

        private bool Validate(
            bool isBaseValid,
            string? key,
            object? model)
        {
            if (model == null)
            {
                return isBaseValid;
            }

            // Use FluentValidation to perform additional validation
            var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
            if (!(this.serviceProvider.GetService(validatorType) is IValidator validator))
            {
                return isBaseValid;
            }

            var validationResult = validator.Validate(new ValidationContext<object>(model));
            foreach (var error in validationResult.Errors)
            {
                var keyName = string.IsNullOrEmpty(key) ? error.PropertyName : $"{key}.{error.PropertyName}";

                if (!ModelState[keyName]?.Errors.Any(e => e.ErrorMessage == error.ErrorMessage) ?? true)
                {
                    ModelState.AddModelError(keyName, error.ErrorMessage);
                }
            }

            return isBaseValid && validationResult.IsValid;
        }
    }
}