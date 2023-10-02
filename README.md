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

Register your validators with the Microsoft DI service container, for instructions on setting that up please see https://docs.fluentvalidation.net/en/latest/di.html.

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

| Property                                    | Default value            | Description                                                                                                                                                                                                                                                                                                                                                                                                            |
|----------------------------------------------|--------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisableBuiltInModelValidation                | `false`                  | Disables the built-in .NET model (data annotations) validation.                                                                                                                                                                                                                                                                                                                                                        |
| ValidationStrategy                           | `ValidationStrategy.All` | Configures the validation strategy. Validation strategy `ValidationStrategy.All` enables asynchronous automatic validation on all controllers inheriting from `ControllerBase`. Validation strategy `ValidationStrategy.Annotations` enables asynchronous automatic validation on controllers inheriting from `ControllerBase` decorated (class or method) with a `FluentValidationAutoValidationAttribute` attribute. |
| EnableBodyBindingSourceAutomaticValidation   | `true`                   | Enables asynchronous automatic validation for parameters bound from the `BindingSource.Body` binding source (typically parameters decorated with the `[FormBody]` attribute).                                                                                                                                                                                                                                          |
| EnableFormBindingSourceAutomaticValidation   | `false`                  | Enables asynchronous automatic validation for parameters bound from the `BindingSource.Form` binding source (typically parameters decorated with the `[FormForm]` attribute).                                                                                                                                                                                                                                          |
| EnableQueryBindingSourceAutomaticValidation  | `true`                   | Enables asynchronous automatic validation for parameters bound from the `BindingSource.Query` binding source (typically parameters decorated with the `[FormQuery]` attribute).                                                                                                                                                                                                                                        |
| EnableCustomBindingSourceAutomaticValidation | `false`                  | Enables asynchronous automatic validation for parameters bound from the `BindingSource.Custom` binding source.                                                                                                                                                                                                                                        |

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    // Disable the built-in .NET model (data annotations) validation.
    configuration.DisableBuiltInModelValidation = true;

    // Only validate controllers decorated with the `FluentValidationAutoValidation` attribute.
    configuration.ValidationStrategy = ValidationStrategy.Annotation;

    // Enable validation for parameters bound from the `BindingSource.Body` binding source.
    configuration.EnableBodyBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from the `BindingSource.Form` binding source.
    configuration.EnableFormBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from the `BindingSource.Query` binding source.
    configuration.EnableQueryBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from 'BindingSource.Custom' sources.
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
