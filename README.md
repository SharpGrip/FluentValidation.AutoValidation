# SharpGrip FluentValidation AutoValidation

## Builds

[![FluentValidation.AutoValidation [Build]](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml/badge.svg)](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=alert_status)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=security_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=coverage)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation)

## Introduction

SharpGrip FluentValidation AutoValidation is an extension of the [FluentValidation](https://github.com/FluentValidation/FluentValidation) library enabling automatic asynchronous validation in MVC
controllers and minimal APIs (endpoints).
The library [FluentValidation.AspNetCore](https://github.com/FluentValidation/FluentValidation.AspNetCore) is no longer being maintained and is unsupported. As a result, support for automatic
validation provided by this library is no longer available.
This library re-introduces this functionality for MVC controllers and introduces automation validation for minimal APIs (endpoints). It enables developers to easily implement automatic validation in
their projects.

## Installation

Register your validators with the Microsoft DI service container, for instructions on setting that up please see https://docs.fluentvalidation.net/en/latest/di.html.

### MVC controllers [![NuGet](https://img.shields.io/nuget/v/SharpGrip.FluentValidation.AutoValidation.Mvc)](https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Mvc)

For MVC controllers reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Mvc` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Mvc).

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation();
```

### Minimal APIs [![NuGet](https://img.shields.io/nuget/v/SharpGrip.FluentValidation.AutoValidation.Endpoints)](https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Endpoints)

For minimal APIs (endpoints) reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Endpoints` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Endpoints).

Enabling minimal API (endpoint) automatic validation can be done on both route groups and routes.

```
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var endpointGroup = app.MapGroup("/some-group").AddFluentValidationAutoValidation();
endpointGroup.MapPost("/", (SomeModel someModel) => $"Hello {someModel.Name}");

app.MapPost("/", (SomeOtherModel someOtherModel) => $"Hello again {someOtherModel.Name}").AddFluentValidationAutoValidation();
```

## Configuration

### MVC controllers

| Property                      | Default value            | Description                                                                                                                                                                                                                                                                                                                                                                                                            |
|-------------------------------|--------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisableBuiltInModelValidation | `false`                  | Disables the built-in model validation.                                                                                                                                                                                                                                                                                                                                                                                |
| ValidationStrategy            | `ValidationStrategy.All` | Configures the validation strategy. Validation strategy `ValidationStrategy.All` enables asynchronous automatic validation on all controllers inheriting from `ControllerBase`. Validation strategy `ValidationStrategy.Annotations` enables asynchronous automatic validation on controllers inheriting from `ControllerBase` decorated (class or method) with a `FluentValidationAutoValidationAttribute` attribute. |

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.DisableDataAnnotationsValidation = true;

    // Only validate controllers decorated with the `FluentValidationAutoValidation` attribute.
    configuration.ValidationStrategy = ValidationStrategy.Annotation;
});
```