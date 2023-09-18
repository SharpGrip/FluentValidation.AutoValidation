using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Xunit;

namespace FluentValidation.AutoValidation.Mvc.Tests.Results;

public class FluentValidationAutoValidationDefaultResultFactoryTest
{
    private static readonly Dictionary<string, string[]> ValidationFailures = new()
    {
        {"Property 1", new[] {"Error message 1"}},
        {"Property 2", new[] {"Error message 2"}},
        {"Property 3", new[] {"Error message 3"}},
    };

    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_DisableBuiltInModelValidation_False()
    {
        var fluentValidationAutoValidationDefaultResultFactory = new FluentValidationAutoValidationDefaultResultFactory();

        var actionContext = Substitute.For<ActionContext>(Substitute.For<HttpContext>(), Substitute.For<RouteData>(), Substitute.For<ActionDescriptor>());
        var actionExecutingContext = Substitute.For<ActionExecutingContext>(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());

        var validationProblemDetails = new ValidationProblemDetails(ValidationFailures);
        var badRequestObjectResult = new BadRequestObjectResult(validationProblemDetails);

        var resultFactoryResult = (BadRequestObjectResult) fluentValidationAutoValidationDefaultResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails);

        Assert.Equal(badRequestObjectResult.Value, resultFactoryResult.Value);
    }
}