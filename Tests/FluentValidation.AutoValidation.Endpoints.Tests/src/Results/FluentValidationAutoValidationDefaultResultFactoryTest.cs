using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Xunit;

namespace FluentValidation.AutoValidation.Endpoints.Tests.Results;

public class FluentValidationAutoValidationDefaultResultFactoryTest
{
    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_DisableBuiltInModelValidation_False()
    {
        var fluentValidationAutoValidationDefaultResultFactory = new FluentValidationAutoValidationDefaultResultFactory();
        var endpointFilterInvocationContext = Substitute.For<EndpointFilterInvocationContext>();

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(TestModel.Parameter1), $"'{nameof(TestModel.Parameter1)}' must be empty."),
            new(nameof(TestModel.Parameter2), $"'{nameof(TestModel.Parameter2)}' must be empty."),
            new(nameof(TestModel.Parameter3), $"'{nameof(TestModel.Parameter3)}' must be empty.")
        };

        var validationResult = new ValidationResult(validationFailures);

        var resultFactoryResult = (ValidationProblem) fluentValidationAutoValidationDefaultResultFactory.CreateResult(endpointFilterInvocationContext, validationResult);

        Assert.Equal(resultFactoryResult.ProblemDetails.Errors, validationResult.ToValidationProblemErrors());
    }

    private class TestModel
    {
        public string? Parameter1 { get; set; }
        public string? Parameter2 { get; set; }
        public string? Parameter3 { get; set; }
    }
}