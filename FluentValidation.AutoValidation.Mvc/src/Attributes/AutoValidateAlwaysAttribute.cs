﻿using System;

namespace SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AutoValidateAlwaysAttribute : Attribute
    {
    }
}