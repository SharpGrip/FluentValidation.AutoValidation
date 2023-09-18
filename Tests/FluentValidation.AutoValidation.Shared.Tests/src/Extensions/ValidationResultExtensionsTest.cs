using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Xunit;

namespace FluentValidation.AutoValidation.Shared.Tests.Extensions;

public class ValidationResultExtensionsTest
{
    [Fact]
    public void Test_GetValidator()
    {
        var validationFailures = new List<ValidationFailure>
        {
            new("Property 1", "Error message 1"),
            new("Property 2", "Error message 2"),
            new("Property 3", "Error message 3"),
        };

        var validationResult = new ValidationResult(validationFailures);

        var validationProblemErrors = validationResult.ToValidationProblemErrors();

        Assert.Equal(validationProblemErrors, ToValidationProblemErrors(validationResult));
    }

    private static Dictionary<string, string[]> ToValidationProblemErrors(ValidationResult validationResult)
    {
        return validationResult.Errors.GroupBy(validationFailure => validationFailure.PropertyName)
            .ToDictionary(validationFailureGrouping => validationFailureGrouping.Key,
                validationFailureGrouping => validationFailureGrouping.Select(validationFailure => validationFailure.ErrorMessage).ToArray());
    }
}