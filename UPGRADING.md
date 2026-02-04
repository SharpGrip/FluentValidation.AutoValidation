# SharpGrip FluentValidation AutoValidation

## Upgrade guide

### Upgrade from v1 to v2

#### Result factories - MVC controllers

```diff
- public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails);
+ public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults);
```

#### Validation interceptors - MVC controllers

```diff
- public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext);
+ public Task<IValidationContext?> BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext, CancellationToken cancellationToken = default);

- public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext);
+ public Task<ValidationResult?> AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext, ValidationResult validationResult, CancellationToken cancellationToken = default);
```

#### Validation interceptors - Minimal APIs (endpoints)

```diff
- public IValidationContext? BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
+ public Task<IValidationContext?> BeforeValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, CancellationToken cancellationToken = default);

- public ValidationResult? AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext);
+ public Task<ValidationResult?> AfterValidation(EndpointFilterInvocationContext endpointFilterInvocationContext, IValidationContext validationContext, ValidationResult validationResult, CancellationToken cancellationToken = default);
```

#### Attributes - MVC controllers

Replace the deprecated `SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes.FluentValidationAutoValidationAttribute` with `SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes.AutoValidationAttribute`.

```diff
- [FluentValidationAutoValidation]
+ [AutoValidation]
```