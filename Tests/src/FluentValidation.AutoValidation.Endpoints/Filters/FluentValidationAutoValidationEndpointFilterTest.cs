// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Endpoints.Filters;

public class FluentValidationAutoValidationEndpointFilterTest
{
    private static readonly Dictionary<string, string[]> ValidationFailures = new()
    {
        {nameof(TestModel.Parameter1), [$"'{nameof(TestModel.Parameter1)}' must be empty."]},
        {nameof(TestModel.Parameter2), [$"'{nameof(TestModel.Parameter2)}' must be empty."]},
        {nameof(TestModel.Parameter3), [$"'{nameof(TestModel.Parameter3)}' must be empty."]}
    };

    [Fact]
    public async Task TestInvokeAsync_ValidatorFound()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var endpointFilterInvocationContext = Substitute.For<EndpointFilterInvocationContext>();

        endpointFilterInvocationContext.HttpContext.Returns(new DefaultHttpContext {RequestServices = serviceProvider});
        endpointFilterInvocationContext.Arguments.Returns(new List<object?> {new TestModel {Parameter1 = "Value 1", Parameter2 = "Value 2", Parameter3 = "Value 3"}});
        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(typeof(TestModel))).Returns(new TestValidator());
        serviceProvider.GetService(typeof(IGlobalValidationInterceptor)).Returns(new GlobalValidationInterceptor());

        var validationFailuresValues = ValidationFailures.Values.ToList();

        var endpointFilter = new FluentValidationAutoValidationEndpointFilter();

        var result = (ValidationProblem) (await endpointFilter.InvokeAsync(endpointFilterInvocationContext, _ => ValueTask.FromResult(new object())!))!;
        var problemDetailsErrorValues = result.ProblemDetails.Errors.ToList();

        Assert.Contains(validationFailuresValues[0].First(), problemDetailsErrorValues[0].Value);
        Assert.Contains(validationFailuresValues[1].First(), problemDetailsErrorValues[1].Value);
        Assert.Contains(validationFailuresValues[2].First(), problemDetailsErrorValues[2].Value);
    }

    [Fact]
    public async Task TestInvokeAsync_ValidatorNotFound()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var endpointFilterInvocationContext = Substitute.For<EndpointFilterInvocationContext>();

        endpointFilterInvocationContext.HttpContext.Returns(new DefaultHttpContext {RequestServices = serviceProvider});
        endpointFilterInvocationContext.Arguments.Returns(new List<object?> {new TestModel {Parameter1 = "Value 1", Parameter2 = "Value 2", Parameter3 = "Value 3"}});
        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(typeof(TestModel))).Returns(null);
        serviceProvider.GetService(typeof(IGlobalValidationInterceptor)).Returns(null);

        var endpointFilter = new FluentValidationAutoValidationEndpointFilter();

        var result = await endpointFilter.InvokeAsync(endpointFilterInvocationContext, _ => ValueTask.FromResult(new object())!);

        Assert.IsType<object>(result);
    }

    private class TestModel
    {
        public string? Parameter1 { get; set; }
        public string? Parameter2 { get; set; }
        public string? Parameter3 { get; set; }
    }

    private class TestValidator : AbstractValidator<TestModel>, IValidatorInterceptor
    {
        public TestValidator()
        {
            RuleFor(x => x.Parameter1).Empty();
            RuleFor(x => x.Parameter2).Empty();
            RuleFor(x => x.Parameter3).Empty();
        }

        public Task<IValidationContext?> BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IValidationContext?>(null);
        }

        public Task<ValidationResult?> AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, ValidationResult validationResult, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ValidationResult?>(null);
        }
    }

    private class GlobalValidationInterceptor : IGlobalValidationInterceptor
    {
        public Task<IValidationContext?> BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IValidationContext?>(null);
        }

        public Task<ValidationResult?> AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, ValidationResult validationResult, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ValidationResult?>(null);
        }
    }
}