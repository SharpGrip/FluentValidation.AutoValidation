using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters;
using Xunit;

namespace FluentValidation.AutoValidation.Endpoints.Tests.Filters;

public class FluentValidationAutoValidationEndpointFilterTest
{
    [Fact]
    public async Task TestInvokeAsync_ValidatorFound()
    {
        var validationFailures = new Dictionary<string, string[]>
        {
            {nameof(TestModel.Parameter1), new[] {$"'{nameof(TestModel.Parameter1)}' must be empty."}},
            {nameof(TestModel.Parameter2), new[] {$"'{nameof(TestModel.Parameter2)}' must be empty."}},
            {nameof(TestModel.Parameter3), new[] {$"'{nameof(TestModel.Parameter3)}' must be empty."}}
        };

        var serviceProvider = Substitute.For<IServiceProvider>();
        var endpointFilterInvocationContext = Substitute.For<EndpointFilterInvocationContext>();

        endpointFilterInvocationContext.Arguments.Returns(new List<object?> {new TestModel {Parameter1 = "Value 1", Parameter2 = "Value 2", Parameter3 = "Value 3"}});
        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(typeof(TestModel))).Returns(new TestValidator());

        var validationFailuresValues = validationFailures.Values.ToList();

        var endpointFilter = new FluentValidationAutoValidationEndpointFilter(serviceProvider);

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

        endpointFilterInvocationContext.Arguments.Returns(new List<object?> {new TestModel {Parameter1 = "Value 1", Parameter2 = "Value 2", Parameter3 = "Value 3"}});
        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(typeof(TestModel))).Returns(null);

        var endpointFilter = new FluentValidationAutoValidationEndpointFilter(serviceProvider);

        var result = await endpointFilter.InvokeAsync(endpointFilterInvocationContext, _ => ValueTask.FromResult(new object())!);

        Assert.IsType<object>(result);
    }

    private class TestModel
    {
        public string? Parameter1 { get; set; }
        public string? Parameter2 { get; set; }
        public string? Parameter3 { get; set; }
    }

    private class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Parameter1).Empty();
            RuleFor(x => x.Parameter2).Empty();
            RuleFor(x => x.Parameter3).Empty();
        }
    }
}

// namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Filters
// {
//     public class FluentValidationAutoValidationEndpointFilter : IEndpointFilter
//     {
//         private readonly IServiceProvider serviceProvider;
//
//         public FluentValidationAutoValidationEndpointFilter(IServiceProvider serviceProvider)
//         {
//             this.serviceProvider = serviceProvider;
//         }
//
//         public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//         {
//             for (var i = 0; i < context.Arguments.Count; i++)
//             {
//                 var argument = context.Arguments[i];
//
//                 if (argument != null && serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
//                 {
//                     var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
//
//                     if (!validationResult.IsValid)
//                     {
//                         var fluentValidationAutoValidationResultFactory = serviceProvider.GetService<IFluentValidationAutoValidationResultFactory>();
//
//                         if (fluentValidationAutoValidationResultFactory != null)
//                         {
//                             return fluentValidationAutoValidationResultFactory.CreateResult(context, validationResult);
//                         }
//
//                         return new FluentValidationAutoValidationDefaultResultFactory().CreateResult(context, validationResult);
//                     }
//                 }
//             }
//
//             return await next(context);
//         }
//     }
// }