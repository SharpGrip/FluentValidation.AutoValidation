// ReSharper disable InconsistentNaming

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddFluentValidationAutoValidation()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation();

        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertNotContainsServiceDescriptor<IObjectModelValidator, FluentValidationAutoValidationObjectModelValidator>(serviceCollection, ServiceLifetime.Singleton);
        AssertContainsServiceDescriptor<IConfigureOptions<MvcOptions>, ConfigureNamedOptions<MvcOptions>>(serviceCollection, ServiceLifetime.Singleton);
    }

    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_OverriddenResultFactory()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation(options => options.OverrideDefaultResultFactoryWith<TestResultFactory>());

        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertNotContainsServiceDescriptor<IObjectModelValidator, FluentValidationAutoValidationObjectModelValidator>(serviceCollection, ServiceLifetime.Singleton);
        AssertContainsServiceDescriptor<IConfigureOptions<MvcOptions>, ConfigureNamedOptions<MvcOptions>>(serviceCollection, ServiceLifetime.Singleton);
    }

    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_DisableBuiltInModelValidation_False()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation(options => { options.DisableBuiltInModelValidation = false; });

        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertNotContainsServiceDescriptor<IObjectModelValidator, FluentValidationAutoValidationObjectModelValidator>(serviceCollection, ServiceLifetime.Singleton);
        AssertContainsServiceDescriptor<IConfigureOptions<MvcOptions>, ConfigureNamedOptions<MvcOptions>>(serviceCollection, ServiceLifetime.Singleton);
    }

    [Fact]
    public void TestAddFluentValidationAutoValidation_WithConfiguration_DisableBuiltInModelValidation_True()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFluentValidationAutoValidation(options => { options.DisableBuiltInModelValidation = true; });

        AssertNotContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, TestResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertContainsServiceDescriptor<IFluentValidationAutoValidationResultFactory, FluentValidationAutoValidationDefaultResultFactory>(serviceCollection, ServiceLifetime.Scoped);
        AssertContainsServiceDescriptor<IObjectModelValidator, FluentValidationAutoValidationObjectModelValidator>(serviceCollection, ServiceLifetime.Singleton);
        AssertContainsServiceDescriptor<IConfigureOptions<MvcOptions>, ConfigureNamedOptions<MvcOptions>>(serviceCollection, ServiceLifetime.Singleton);
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

    // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    private static void AssertNotContainsServiceDescriptor<TService, TImplementation>(ServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
    {
        Assert.DoesNotContain(serviceCollection, serviceDescriptor => serviceDescriptor.ServiceType == typeof(TService) &&
                                                                      (serviceDescriptor.ImplementationType == typeof(TImplementation) ||
                                                                       serviceDescriptor.ImplementationFactory?.Method.ReturnType == typeof(TImplementation) ||
                                                                       serviceDescriptor.ImplementationInstance?.GetType() == typeof(TImplementation)) &&
                                                                      serviceDescriptor.Lifetime == serviceLifetime);
    }

    private class TestResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
        {
            return new OkResult();
        }
    }
}