// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Results;

public class FluentValidationAutoValidationDefaultResultFactoryTest
{
    private static readonly Dictionary<string, string[]> ValidationFailures = new()
    {
        {"Property 1", ["Error message 1"]},
        {"Property 2", ["Error message 2"]},
        {"Property 3", ["Error message 3"]},
    };

    [Fact]
    public async Task TestAddFluentValidationAutoValidation_WithConfiguration_DisableBuiltInModelValidation_False()
    {
        var fluentValidationAutoValidationDefaultResultFactory = new FluentValidationAutoValidationDefaultResultFactory();

        var actionContext = Substitute.For<ActionContext>(Substitute.For<HttpContext>(), Substitute.For<RouteData>(), Substitute.For<ActionDescriptor>());
        var actionExecutingContext = Substitute.For<ActionExecutingContext>(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());

        var validationProblemDetails = new ValidationProblemDetails(ValidationFailures);
        var badRequestObjectResult = new BadRequestObjectResult(validationProblemDetails);
        var validationResults = new Dictionary<IValidationContext, ValidationResult>();

        var resultFactoryResult = (BadRequestObjectResult?) await fluentValidationAutoValidationDefaultResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails, validationResults);

        Assert.Equal(badRequestObjectResult.Value, resultFactoryResult?.Value);
    }
}