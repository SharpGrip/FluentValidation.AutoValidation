using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Validation;

public class FluentValidationAutoValidationObjectModelValidatorTest
{
    [Fact]
    public void TestGetValidationVisitor()
    {
        var modelMetadataProvider = Substitute.For<IModelMetadataProvider>();
        var modelMetadataProviders = Substitute.For<IList<IModelValidatorProvider>>();
        var actionContext = Substitute.For<ActionContext>();
        var modelValidatorProvider = Substitute.For<IModelValidatorProvider>();
        var validatorCache = Substitute.For<ValidatorCache>();

        var fluentValidationAutoValidationObjectModelValidator = new FluentValidationAutoValidationObjectModelValidator(modelMetadataProvider, modelMetadataProviders, true);

        Assert.IsType<FluentValidationAutoValidationValidationVisitor>(
            fluentValidationAutoValidationObjectModelValidator.GetValidationVisitor(actionContext, modelValidatorProvider, validatorCache, modelMetadataProvider, null));
    }
}