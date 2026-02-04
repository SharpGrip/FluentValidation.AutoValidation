using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Configuration;

public class AutoValidationMvcConfigurationTest
{
    [Fact]
    public void TestOverrideDefaultResultFactoryWith()
    {
        var autoValidationMvcConfiguration = new AutoValidationMvcConfiguration
        {
            DisableBuiltInModelValidation = true
        };

        autoValidationMvcConfiguration.OverrideDefaultResultFactoryWith<TestResultFactory>();

        Assert.True(autoValidationMvcConfiguration.DisableBuiltInModelValidation);
        Assert.Equal(typeof(TestResultFactory), autoValidationMvcConfiguration.OverriddenResultFactory);
    }

    private class TestResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults)
        {
            return Task.FromResult<IActionResult?>(new OkResult());
        }
    }
}