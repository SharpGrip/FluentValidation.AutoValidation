using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Validation;

public class FluentValidationAutoValidationValidationVisitorTest
{
    [Fact]
    public void TestGetValidationVisitor()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var modelMetadataProvider = Substitute.For<IModelMetadataProvider>();
        var actionContext = Substitute.For<ActionContext>();
        var modelValidatorProvider = Substitute.For<IModelValidatorProvider>();
        var validatorCache = Substitute.For<ValidatorCache>();

        var fluentValidationAutoValidationObjectModelValidator =
            new FluentValidationAutoValidationValidationVisitor(
                serviceProvider,
                actionContext,
                modelValidatorProvider,
                validatorCache,
                modelMetadataProvider,
                null,
                true);

#if NETCOREAPP3_1
        Assert.True(fluentValidationAutoValidationObjectModelValidator.Validate(null, null, new TestModel(), true));
#else
        Assert.True(fluentValidationAutoValidationObjectModelValidator.Validate(null, null, new TestModel(), true, null));
#endif
    }

    private class TestModel
    {
    }
}