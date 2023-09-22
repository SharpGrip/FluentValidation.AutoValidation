using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Endpoints.Configuration;

public class AutoValidationEndpointsConfigurationTest
{
    [Fact]
    public void TestOverrideDefaultResultFactoryWith()
    {
        var autoValidationEndpointsConfiguration = new AutoValidationEndpointsConfiguration();

        autoValidationEndpointsConfiguration.OverrideDefaultResultFactoryWith<TestResultFactory>();

        Assert.Equal(typeof(TestResultFactory), autoValidationEndpointsConfiguration.OverriddenResultFactory);
    }

    private class TestResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
        {
            return TypedResults.Empty;
        }
    }
}