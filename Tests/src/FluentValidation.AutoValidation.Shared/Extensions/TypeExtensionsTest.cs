using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Shared.Extensions;

public class TypeExtensionsTest
{
    [Theory]
    [InlineData(typeof(TestModelEnum))]
    [InlineData(typeof(string))]
    [InlineData(typeof(char))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(int))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(nint))]
    [InlineData(typeof(nuint))]
    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(double))]
    [InlineData(typeof(float))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(Dictionary<int, int>))]
    [InlineData(typeof(Array))]
    [InlineData(null)]
    public void Test_IsCustomType_Negative(Type? type)
    {
        Assert.False(type.IsCustomType(), $"Type {type?.Name} was considered as custom type");
    }

    [Theory]
    [InlineData(typeof(TestModelClass))]
    [InlineData(typeof(TestModelRecord))]
    public void Test_IsCustomType_Positive(Type? type)
    {
        Assert.True(type.IsCustomType(), $"Type {type?.Name} was not considered as custom type");
    }

    [Fact]
    public void Test_HasCustomAttribute()
    {
        Assert.True(typeof(TestModelClass).HasCustomAttribute<AutoValidationAttribute>());
        Assert.False(typeof(TestModelClass).HasCustomAttribute<AutoValidateNeverAttribute>());
        Assert.True(typeof(TestModelRecord).HasCustomAttribute<AutoValidateNeverAttribute>());
        Assert.False(typeof(TestModelRecord).HasCustomAttribute<AutoValidationAttribute>());
    }

    [AutoValidation]
    private class TestModelClass;

    [AutoValidateNever]
    private record TestModelRecord;

    private enum TestModelEnum;
}