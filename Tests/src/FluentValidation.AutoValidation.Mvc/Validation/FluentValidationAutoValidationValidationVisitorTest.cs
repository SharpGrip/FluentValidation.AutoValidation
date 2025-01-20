using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Validation;

public class FluentValidationAutoValidationValidationVisitorTest
{
    [Fact]
    public void TestGetValidationVisitor()
    {
        var modelMetadataProvider = Substitute.For<IModelMetadataProvider>();
        var actionContext = Substitute.For<ActionContext>();
        var httpContext = Substitute.For<HttpContext>();
        var routeData = Substitute.For<RouteData>();
        var actionDescriptor = Substitute.For<ActionDescriptor>();
        actionContext.HttpContext = httpContext;
        actionContext.RouteData = routeData;
        actionContext.ActionDescriptor = actionDescriptor;
        var modelValidatorProvider = Substitute.For<IModelValidatorProvider>();
        var validatorCache = Substitute.For<ValidatorCache>();

        var fluentValidationAutoValidationObjectModelValidator =
            new FluentValidationAutoValidationValidationVisitor(
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