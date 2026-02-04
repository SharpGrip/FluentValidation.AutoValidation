namespace SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors
{
    /// <summary>
    /// Allows intercepting and altering of the validation process by implementing this interface on a validator.
    ///
    /// The interceptor methods of instances of this interface will only get called when the implementing validator gets validated.
    /// </summary>
    public interface IValidatorInterceptor : IGlobalValidationInterceptor;
}