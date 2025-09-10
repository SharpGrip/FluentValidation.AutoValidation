using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Validation
{
    public class FluentValidationAutoValidationValidationVisitor : ValidationVisitor
    {
        private readonly ActionContext actionContext;
        private readonly bool disableBuiltInModelValidation;

        public FluentValidationAutoValidationValidationVisitor(
            ActionContext actionContext,
            IModelValidatorProvider validatorProvider,
            ValidatorCache validatorCache,
            IModelMetadataProvider metadataProvider,
            ValidationStateDictionary? validationState,
            bool disableBuiltInModelValidation)
            : base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
        {
            this.actionContext = actionContext;
            this.disableBuiltInModelValidation = disableBuiltInModelValidation;
        }

        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel)
        {
            // If built in model validation is disabled return true for later validation in the action filter.
            bool isBaseValid = disableBuiltInModelValidation || base.Validate(metadata, key, model, alwaysValidateAtTopLevel);
            return ValidateAsync(isBaseValid, key, model).GetAwaiter().GetResult();
        }

#if !NETCOREAPP3_1
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel, object? container)
        {
            // If built in model validation is disabled return true for later validation in the action filter.
            bool isBaseValid = disableBuiltInModelValidation || base.Validate(metadata, key, model, alwaysValidateAtTopLevel, container);
            return ValidateAsync(isBaseValid, key, model).GetAwaiter().GetResult();
        }
#endif

        private async Task<bool> ValidateAsync(
            bool defaultValue,
            string? key,
            object? model)
        {
            if (model == null)
            {
                return defaultValue;
            }

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                null);

            var validationResult = await FluentValidationHelper.ValidateWithFluentValidationAsync(
                actionContext.HttpContext.RequestServices,
                model,
                actionExecutingContext);
            if (validationResult == null)
            {
                return defaultValue;
            }

            foreach (var error in validationResult.Errors)
            {
                var keyName = string.IsNullOrEmpty(key) ? error.PropertyName : $"{key}.{error.PropertyName}";
                if (!this.ModelState[keyName]?.Errors.Any(e => e.ErrorMessage == error.ErrorMessage) ?? true)
                {
                    this.ModelState.AddModelError(keyName, error.ErrorMessage);
                }
            }

            return defaultValue && validationResult.IsValid;
        }
    }
}