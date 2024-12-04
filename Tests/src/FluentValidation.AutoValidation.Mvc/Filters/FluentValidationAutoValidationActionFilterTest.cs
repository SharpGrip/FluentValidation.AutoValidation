using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Configuration;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Xunit;
using IValidatorInterceptor = SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors.IValidatorInterceptor;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Filters;

public class FluentValidationAutoValidationActionFilterTest
{
    [Fact]
    public async Task TestOnActionExecutionAsync()
    {
        var actionArguments = new Dictionary<string, object?>
        {
            {
                nameof(TestModel), new TestModel
                {
                    Parameter1 = "Value 1",
                    Parameter2 = "Value 2",
                    Parameter3 = "Value 3"
                }
            },
        };
        var controllerActionDescriptor = new ControllerActionDescriptor
        {
            Parameters = new List<ParameterDescriptor>
            {
                new()
                {
                    Name = nameof(TestModel),
                    ParameterType = typeof(TestModel),
                    BindingInfo = new BindingInfo {BindingSource = BindingSource.Body}
                }
            }
        };
        var validationFailures = new Dictionary<string, string[]>
        {
            {nameof(TestModel.Parameter1), [$"'{nameof(TestModel.Parameter1)}' must be empty."]},
            {nameof(TestModel.Parameter2), [$"'{nameof(TestModel.Parameter2)}' must be empty."]},
            {nameof(TestModel.Parameter3), [$"'{nameof(TestModel.Parameter3)}' must be empty."]}
        };

        var validationProblemDetails = new ValidationProblemDetails(validationFailures);
        var modelStateDictionary = new ModelStateDictionary();

        var serviceProvider = Substitute.For<IServiceProvider>();
        var problemDetailsFactory = Substitute.For<ProblemDetailsFactory>();
        var fluentValidationAutoValidationResultFactory = Substitute.For<IFluentValidationAutoValidationResultFactory>();
        var autoValidationMvcConfiguration = Substitute.For<IOptions<AutoValidationMvcConfiguration>>();
        var httpContext = Substitute.For<HttpContext>();
        var controller = Substitute.For<TestController>();
        var actionContext = Substitute.For<ActionContext>(httpContext, Substitute.For<RouteData>(), controllerActionDescriptor, modelStateDictionary);
        var actionExecutingContext = Substitute.For<ActionExecutingContext>(actionContext, new List<IFilterMetadata>(), actionArguments, new object());
        var actionExecutedContext = Substitute.For<ActionExecutedContext>(actionContext, new List<IFilterMetadata>(), new object());

        serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(typeof(TestModel))).Returns(new TestValidator());
        serviceProvider.GetService(typeof(IGlobalValidationInterceptor)).Returns(new GlobalValidationInterceptor());
        serviceProvider.GetService(typeof(ProblemDetailsFactory)).Returns(problemDetailsFactory);

        problemDetailsFactory.CreateValidationProblemDetails(httpContext, modelStateDictionary).Returns(validationProblemDetails);
        fluentValidationAutoValidationResultFactory.CreateActionResult(actionExecutingContext, validationProblemDetails).Returns(new BadRequestObjectResult(validationProblemDetails));
        httpContext.RequestServices.Returns(serviceProvider);
        actionExecutingContext.Controller.Returns(controller);
        actionExecutingContext.ActionDescriptor = controllerActionDescriptor;
        actionExecutingContext.ActionArguments.Returns(actionArguments);
        autoValidationMvcConfiguration.Value.Returns(new AutoValidationMvcConfiguration());

        var actionFilter = new FluentValidationAutoValidationActionFilter(fluentValidationAutoValidationResultFactory, autoValidationMvcConfiguration);

        await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

        var modelStateDictionaryValues = modelStateDictionary.Values.ToList();
        var validationFailuresValues = validationFailures.Values.ToList();
        var badRequestObjectResult = (BadRequestObjectResult) actionExecutingContext.Result!;
        var badRequestObjectResultValidationProblemDetails = (ValidationProblemDetails) badRequestObjectResult.Value!;

        Assert.Contains(validationFailuresValues[0].First(), modelStateDictionaryValues[0].Errors.Select(error => error.ErrorMessage));
        Assert.Contains(validationFailuresValues[1].First(), modelStateDictionaryValues[1].Errors.Select(error => error.ErrorMessage));
        Assert.Contains(validationFailuresValues[2].First(), modelStateDictionaryValues[2].Errors.Select(error => error.ErrorMessage));

        Assert.Contains(validationFailuresValues[0].First(), badRequestObjectResultValidationProblemDetails.Errors[nameof(TestModel.Parameter1)][0]);
        Assert.Contains(validationFailuresValues[1].First(), badRequestObjectResultValidationProblemDetails.Errors[nameof(TestModel.Parameter2)][0]);
        Assert.Contains(validationFailuresValues[2].First(), badRequestObjectResultValidationProblemDetails.Errors[nameof(TestModel.Parameter3)][0]);
    }

    public class TestController : ControllerBase
    {
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

        public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
        {
            return null;
        }

        public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
        {
            return null;
        }
    }

    private class GlobalValidationInterceptor : IGlobalValidationInterceptor
    {
        public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
        {
            return null;
        }

        public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
        {
            return null;
        }
    }
}