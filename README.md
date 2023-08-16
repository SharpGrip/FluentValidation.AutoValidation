# SharpGrip FluentValidation AutoValidation

## Builds

[![FluentValidation.AutoValidation [Build]](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml/badge.svg)](https://github.com/SharpGrip/FluentValidation.AutoValidation/actions/workflows/Build.yaml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=alert_status)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=security_rating)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation) \
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SharpGrip_FluentValidation.AutoValidation&metric=coverage)](https://sonarcloud.io/summary/overall?id=SharpGrip_FluentValidation.AutoValidation)

## Introduction

SharpGrip FluentValidation AutoValidation is an extension of the [FluentValidation](https://github.com/FluentValidation/FluentValidation) library enabling automatic asynchronous validation in MVC controllers and minimal APIs (endpoints).
The library [FluentValidation.AspNetCore](https://github.com/FluentValidation/FluentValidation.AspNetCore) is no longer being maintained and is unsupported. As a result, support for automatic validation provided by this library is no longer available.
This library re-introduces this functionality for MVC controllers and introduces automation validation for minimal APIs (endpoints). It enables developers to easily implement automatic validation in their projects.

## Installation

Register your validators with the Microsoft DI service container, for reference please see https://docs.fluentvalidation.net/en/latest/di.html.

### MVC controllers
For MVC controllers reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Mvc` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Mvc).

### Minimal APIs
For minimal APIs (endpoints) reference NuGet package `SharpGrip.FluentValidation.AutoValidation.Endpoints` (https://www.nuget.org/packages/SharpGrip.FluentValidation.AutoValidation.Endpoints).

## Usage

### MVC controllers

```
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

builder.Services.AddFluentValidationAutoValidation();
```

### Minimal APIs (endpoints)

Enabling minimal API (endpoint) automatic validation can be done on both route groups and routes.

```
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var endpointGroup = app.MapGroup("/some-group").AddFluentValidationAutoValidation();
endpointGroup.MapPost("/", (TestCreateModel testCreateModel) => $"Hello {testCreateModel.Name}");

app.MapPost("/", (TestCreateModel testCreateModel) => $"Hello again {testCreateModel.Name}").AddFluentValidationAutoValidation();
```