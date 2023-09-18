using System;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Xunit;

namespace FluentValidation.AutoValidation.Shared.Tests.Extensions;

public class ServiceProviderExtensionsTest
{
    [Fact]
    public void Test_GetValidator()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();

        var testModel = new TestModel();
        var testModelValidator = new TestModelValidator();

        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(testModel.GetType())).Returns(testModelValidator);

        var validator = serviceProvider.GetValidator(testModel.GetType());

        Assert.Equal(testModelValidator, validator);
    }

    private class TestModel
    {
    }

    private class TestModelValidator
    {
    }
}