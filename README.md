# SharpGrip FluentValidation AutoValidation

## Builds

[![FluentValidation.AutoValidation [Build]](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml/badge.svg)](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=alert_status)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=security_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=coverage)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation)

## Introduction

SharpGrip FluentValidation AutoValidation is an extension of the [FluentValidation](https://github.com/FluentValidation/FluentValidation) (v10+) library enabling automatic asynchronous validation in MVC controllers and minimal APIs (endpoints).
The library [FluentValidation.AspNetCore](https://github.com/FluentValidation/FluentValidation.AspNetCore) is no longer being maintained and is unsupported. As a result, support for automatic validation provided by this library is no longer available.
This library re-introduces this functionality for MVC controllers and introduces automatic validation for minimal APIs (endpoints). It enables developers to easily implement automatic validation in their projects.

## Installation

### Validator registration

To enable this library to automatically resolve and invoke validators for your models, you must register your validators with the Dependency Injection (DI) service container.

#### Manual registration

Manually register your validator with the service container:

```
services.AddScoped<IValidator<User>, UserValidator>();
```

#### Automatic registration

Automatically register all validators from the assembly containing your UserValidator:

```
services.AddValidatorsFromAssemblyContaining<UserValidator>();
```

For more instructions on setting that up please see https://docs.fluentvalidation.net/en/latest/di.html.

### MVC controllers [![NuGet](https://img.shields.io/nuget/v/SharpGrip.FluentValidation.AutoValidation.Mvc)](https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Mvc)

For MVC controllers reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Mvc` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Mvc).

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation();
```

### Minimal APIs (endpoints) [![NuGet](https://img.shields.io/nuget/v/SharpGrip.FluentValidation.AutoValidation.Endpoints)](https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Endpoints)

For minimal APIs (endpoints) reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Endpoints` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Endpoints).

Enabling minimal API (endpoint) automatic validation can be done on both route groups and routes.

```
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();
var endpointGroup = app.MapGroup("/some-group").AddFluentValidationAutoValidation();
endpointGroup.MapPost("/", (SomeModel someModel) => $"Hello {someModel.Name}");

app.MapPost("/", (SomeOtherModel someOtherModel) => $"Hello again {someOtherModel.Name}").AddFluentValidationAutoValidation();
```

## Configuration

### MVC controllers

| Property                                     | Default value            | Description                                                                                                                                                                                                                                                                                                                                                                                              |
|----------------------------------------------|--------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisableBuiltInModelValidation                | `false`                  | Disables the built-in .NET model (data annotations) validation.                                                                                                                                                                                                                                                                                                                                          |
| ValidationStrategy                           | `ValidationStrategy.All` | Configures the validation strategy. Validation strategy `ValidationStrategy.All` enables asynchronous automatic validation on all controllers inheriting from `ControllerBase`. Validation strategy `ValidationStrategy.Annotations` enables asynchronous automatic validation on controllers inheriting from `ControllerBase` decorated (class or method) with a `[AutoValidationAttribute]` attribute. |
| EnableBodyBindingSourceAutomaticValidation   | `true`                   | Enables asynchronous automatic validation for parameters bound from `BindingSource.Body` binding sources (typically parameters decorated with the `[FromBody]` attribute).                                                                                                                                                                                                                               |
| EnableFormBindingSourceAutomaticValidation   | `false`                  | Enables asynchronous automatic validation for parameters bound from `BindingSource.Form` binding sources (typically parameters decorated with the `[FromForm]` attribute).                                                                                                                                                                                                                               |
| EnableQueryBindingSourceAutomaticValidation  | `true`                   | Enables asynchronous automatic validation for parameters bound from `BindingSource.Query` binding sources (typically parameters decorated with the `[FromQuery]` attribute).                                                                                                                                                                                                                             |
| EnablePathBindingSourceAutomaticValidation   | `false`                  | Enables asynchronous automatic validation for parameters bound from `BindingSource.Path` binding sources (typically parameters decorated with the `[FromRoute]` attribute).                                                                                                                                                                                                                              |
| EnableCustomBindingSourceAutomaticValidation | `false`                  | Enables asynchronous automatic validation for parameters bound from `BindingSource.Custom` binding sources.                                                                                                                                                                                                                                                                                              |
| EnableNullBindingSourceAutomaticValidation   | `false`                  | Enables asynchronous automatic validation for parameters not bound from any binding source (typically parameters without a declared or inferred binding source).                                                                                                                                                                                                                                         |

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    // Disable the built-in .NET model (data annotations) validation.
    configuration.DisableBuiltInModelValidation = true;

    // Only validate controllers decorated with the `AutoValidation` attribute.
    configuration.ValidationStrategy = ValidationStrategy.Annotations;

    // Enable validation for parameters bound from `BindingSource.Body` binding sources.
    configuration.EnableBodyBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Form` binding sources.
    configuration.EnableFormBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Query` binding sources.
    configuration.EnableQueryBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Path` binding sources.
    configuration.EnablePathBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from 'BindingSource.Custom' binding sources.
    configuration.EnableCustomBindingSourceAutomaticValidation = true;

    // Replace the default result factory with a custom implementation.
    configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
});

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        return new BadRequestObjectResult(new {Title = "Validation errors", ValidationErrors = validationProblemDetails?.Errors});
    }
}
```

### Minimal APIs (endpoints)

```
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    // Replace the default result factory with a custom implementation.
    configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
});

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var validationProblemErrors = validationResult.ToValidationProblemErrors();

        return Results.ValidationProblem(validationProblemErrors, "Some details text.", "Some instance text.", (int) HttpStatusCode.BadRequest, "Some title.");
    }
}
```

## Validation attributes

### MVC controllers

Customizing automatic validation behavior is achievable through the use of attributes.

The `[AutoValidateAlways]` attribute can be applied to a controller parameter, compelling automatic validation to disregard the validation check for a valid binding source.
This proves useful when the `ApiBehaviorOptions.SuppressInferBindingSourcesForParameters` option is enabled, and a custom model is used, with parameters bound from multiple binding sources.

The `[AutoValidateNever]` attribute can be placed on a controller class, controller method, or controller parameter, instructing automatic validation to be skipped.

## Validation interceptors

**Note:** Using validation interceptors is considered to be an advanced feature and is not needed for most use cases.

Validation interceptors provide a mechanism for intercepting and modifying the validation process. This can be achieved through two distinct approaches:

Global validation interceptor:
Create a custom class that implements the `IGlobalValidationInterceptor` interface and register it with the service provider.

Per validator interceptor:
Implement the `IValidatorInterceptor` interface directly on a specific validator.

In the validation process, both the global and the validator interceptors are resolved and invoked (if they exist), thereby establishing a miniature pipeline of validation interceptors:

```
==> IValidatorInterceptor.BeforeValidation()
==> IGlobalValidationInterceptor.BeforeValidation()

Validation

==> IValidatorInterceptor.AfterValidation()
==> IGlobalValidationInterceptor.AfterValidation()
```

Both interfaces define a `BeforeValidation` and a `AfterValidation` method.

The `BeforeValidation` method gets called before validation and allows you to return a custom `IValidationContext` which gets passed to the validator.
In case you return `null` the default `IValidationContext` will be passed to the validator.

The `AfterValidation` method gets called after validation and allows you to return a custom `IValidationResult` which gets passed to the `IFluentValidationAutoValidationResultFactory`.
In case you return `null` the default `IValidationResult` will be passed to the `IFluentValidationAutoValidationResultFactory`.

### MVC controllers

```
// Example of a global validation interceptor.
builder.Services.AddTransient<IGlobalValidationInterceptor, CustomGlobalValidationInterceptor>();

public class CustomGlobalValidationInterceptor : IGlobalValidationInterceptor
{
    public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Return a custom `IValidationContext` or null.
        return null;
    }

    public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Return a custom `ValidationResult` or null.
        return null;
    }
}

// Example of a single validator interceptor.
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
        // Return a custom `IValidationContext` or null.
        return null;
    }

    public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Return a custom `ValidationResult` or null.
        return null;
    }
}
```

### Minimal APIs (endpoints)

```
// Example of a global validation interceptor.
builder.Services.AddTransient<IGlobalValidationInterceptor, CustomGlobalValidationInterceptor>();

public class CustomGlobalValidationInterceptor : IGlobalValidationInterceptor
{
    public IValidationContext? BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext)
    {
        // Return a custom `IValidationContext` or null.
        return null;
    }

    public ValidationResult? AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext)
    {
        // Return a custom `ValidationResult` or null.
        return null;
    }
}

// Example of a single validator interceptor.
private class TestValidator : AbstractValidator<TestModel>, IValidatorInterceptor
{
    public TestValidator()
    {
        RuleFor(x => x.Parameter1).Empty();
        RuleFor(x => x.Parameter2).Empty();
        RuleFor(x => x.Parameter3).Empty();
    }

    public IValidationContext? BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext)
    {
        // Return a custom `IValidationContext` or null.
        return null;
    }

    public ValidationResult? AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext)
    {
        // Return a custom `ValidationResult` or null.
        return null;
    }
}
```
