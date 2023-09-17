using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Xunit;

namespace FluentValidation.AutoValidation.Mvc.Tests.Configuration
{
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
            public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
            {
                return new OkResult();
            }
        }
    }
}