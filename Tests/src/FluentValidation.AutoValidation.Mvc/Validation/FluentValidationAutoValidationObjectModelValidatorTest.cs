using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Validation;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Mvc.Validation;

public class FluentValidationAutoValidationObjectModelValidatorTest
{
    [Fact]
    public void TestGetValidationVisitor()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var modelMetadataProvider = Substitute.For<IModelMetadataProvider>();
        var modelMetadataProviders = Substitute.For<IList<IModelValidatorProvider>>();
        var actionContext = Substitute.For<ActionContext>();
        var modelValidatorProvider = Substitute.For<IModelValidatorProvider>();
        var validatorCache = Substitute.For<ValidatorCache>();

        var fluentValidationAutoValidationObjectModelValidator = new FluentValidationAutoValidationObjectModelValidator(
            serviceProvider, modelMetadataProvider, modelMetadataProviders, true);

        Assert.IsType<FluentValidationAutoValidationValidationVisitor>(
            fluentValidationAutoValidationObjectModelValidator.GetValidationVisitor(actionContext, modelValidatorProvider, validatorCache, modelMetadataProvider, null));
    }

    [Fact]
    public void TryValidateModel_WithInvalidModel_ShouldUpdateModelState()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddControllersWithViews();
        serviceCollection.AddFluentValidationAutoValidation();
        serviceCollection.AddTransient<IValidator<Test1Controller.Action1ViewModel>, Test1Controller.Action1ViewModel.Action1ViewModelValidator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new Test1Controller.Action1ViewModel
        {
            ValueMustEqual1 = 0 // Invalid value to trigger validation error
        };

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        var routeData = Substitute.For<RouteData>();
        var actionDescriptor = Substitute.For<ControllerActionDescriptor>();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        var controller = new Test1Controller
        {
            ControllerContext = new ControllerContext(actionContext)
        };

        // Act
        bool result = controller.TryValidateModel(viewModel);

        // Assert
        Assert.False(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(Test1Controller.Action1ViewModel.ValueMustEqual1)));
        var modelError = controller.ModelState[nameof(Test1Controller.Action1ViewModel.ValueMustEqual1)]!.Errors.Single();
        Assert.NotNull(modelError);
        Assert.Equal("'ValueMustEqual1' must be equal to '1'.", modelError.ErrorMessage);
    }

    public class Test1Controller : Controller
    {
        public class Action1ViewModel
        {
            public int ValueMustEqual1 { get; set; }

            internal class Action1ViewModelValidator : AbstractValidator<Action1ViewModel>
            {
                public Action1ViewModelValidator()
                {
                    this.RuleFor(x => x.ValueMustEqual1)
                        .Equal(1)
                        .WithName(nameof(ValueMustEqual1));
                }
            }
        }
    }
}