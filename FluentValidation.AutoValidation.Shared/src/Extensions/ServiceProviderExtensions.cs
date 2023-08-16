using System;
using FluentValidation;

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static object? GetValidator(this IServiceProvider serviceProvider, Type type)
        {
            return serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(type));
        }   
    }
}