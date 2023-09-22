using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using Xunit;

namespace FluentValidation.AutoValidation.Endpoints.Tests.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddFluentValidationAutoValidation()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation();

        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
    }

    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_OverriddenResultFactory()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation(options => options.OverrideDefaultResultFactoryWith<TestResultFactory>());

        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
    }

    // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    private static void AssertContainsServiceDescriptor<TService, TImplementation>(ServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
    {
        Assert.Contains(serviceCollection, serviceDescriptor => serviceDescriptor.ServiceType == typeof(TService) &&
                                                                (serviceDescriptor.ImplementationType == typeof(TImplementation) ||
                                                                 serviceDescriptor.ImplementationFactory?.Method.ReturnType == typeof(TImplementation) ||
                                                                 serviceDescriptor.ImplementationInstance?.GetType() == typeof(TImplementation)) &&
                                                                serviceDescriptor.Lifetime == serviceLifetime);
    }
    // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

    // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    private static void AssertNotContainsServiceDescriptor<TService, TImplementation>(ServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
    {
        Assert.DoesNotContain(serviceCollection, serviceDescriptor => serviceDescriptor.ServiceType == typeof(TService) &&
                                                                      (serviceDescriptor.ImplementationType == typeof(TImplementation) ||
                                                                       serviceDescriptor.ImplementationFactory?.Method.ReturnType == typeof(TImplementation) ||
                                                                       serviceDescriptor.ImplementationInstance?.GetType() == typeof(TImplementation)) &&
                                                                      serviceDescriptor.Lifetime == serviceLifetime);
    }
    // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

    private class TestResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
        {
            return TypedResults.Ok();
        }
    }
}