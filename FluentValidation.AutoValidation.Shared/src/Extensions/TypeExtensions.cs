using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsCustomType(this Type? type)
        {
            if (type == null || type.IsEnum || type.IsPrimitive)
            {
                return false;
            }

            var builtInTypes = new HashSet<Type>
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(DateOnly),
                typeof(TimeOnly),
                typeof(Uri),
                typeof(Guid),
                typeof(Enum)
            };

            if (builtInTypes.Contains(type))
            {
                return false;
            }

            return type.IsClass || type.IsValueType;
        }

        public static bool HasCustomAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(TAttribute));
        }

        public static bool InheritsFromTypeWithNameEndingIn(this Type type, string name)
        {
            while (type.BaseType != null)
            {
                type = type.BaseType;

                if (type.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}