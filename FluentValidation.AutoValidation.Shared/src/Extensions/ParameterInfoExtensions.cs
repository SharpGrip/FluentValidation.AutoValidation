﻿using System;
using System.Linq;
using System.Reflection;

namespace SharpGrip.FluentValidation.AutoValidation.Shared.Extensions
{
    public static class ParameterInfoExtensions
    {
        public static bool HasCustomAttribute<TAttribute>(this ParameterInfo parameterInfo) where TAttribute : Attribute
        {
            return parameterInfo.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(TAttribute));
        }

        public static TAttribute? GetCustomAttribute<TAttribute>(this ParameterInfo parameterInfo) where TAttribute : Attribute
        {
            return parameterInfo.GetCustomAttributes(true).FirstOrDefault(attribute => attribute is TAttribute) as TAttribute;
        }
    }
}