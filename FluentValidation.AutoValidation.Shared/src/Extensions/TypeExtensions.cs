using System;
using System.Collections;
using System.Linq;

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsCustomType(this Type? type)
        {
            return type != null 
                && type.IsClass
                && !type.IsEnum 
                && !type.IsValueType
                && !type.IsPrimitive
                && type != typeof(string);
        }

        public static bool HasCustomAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(TAttribute));
        }
    }
}