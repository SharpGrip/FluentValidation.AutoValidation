using System;
using FluentValidation;
#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class ServiceProviderExtensions
    {
#if NET7_0_OR_GREATER
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IValidator<>))]
        [UnconditionalSuppressMessage("Aot", "IL3050", Justification = "Validators should be preserved by the compiler if the user has marked them with [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(YourValidator!)] or similar.")]
#endif
        public static object? GetValidator(this IServiceProvider serviceProvider, Type type)
        {
            return serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(type));
        }
    }
}