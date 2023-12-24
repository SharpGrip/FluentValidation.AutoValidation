using System;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Xunit;

namespace SharpGrip.FluentValidation.AutoValidation.Tests.FluentValidation.AutoValidation.Shared.Extensions;

public class TypeExtensionsTest
{
    [Fact]
    public void Test_IsCustomType()
    {
        Assert.True(typeof(TestModelClass).IsCustomType());
        Assert.True(typeof(TestModelRecord).IsCustomType());
        Assert.False(typeof(TestModelEnum).IsCustomType());
        Assert.False(typeof(Enum).IsCustomType());
        Assert.False(typeof(string).IsCustomType());
        Assert.False(typeof(char).IsCustomType());
        Assert.False(typeof(short).IsCustomType());
        Assert.False(typeof(ushort).IsCustomType());
        Assert.False(typeof(int).IsCustomType());
        Assert.False(typeof(uint).IsCustomType());
        Assert.False(typeof(long).IsCustomType());
        Assert.False(typeof(ulong).IsCustomType());
        Assert.False(typeof(double).IsCustomType());
        Assert.False(typeof(float).IsCustomType());
        Assert.False(typeof(decimal).IsCustomType());
        Assert.False(typeof(byte).IsCustomType());
        Assert.False(typeof(sbyte).IsCustomType());
        Assert.False(typeof(DateTime).IsCustomType());
        Assert.False(typeof(DateTimeOffset).IsCustomType());
        Assert.False(typeof(TimeSpan).IsCustomType());
        Assert.False(typeof(Guid).IsCustomType());
        Assert.False(typeof(DateOnly).IsCustomType());
        Assert.False(typeof(TimeOnly).IsCustomType());
    }

    [Fact]
    public void Test_HasCustomAttribute()
    {
        Assert.True(typeof(TestModelClass).HasCustomAttribute<AutoValidationAttribute>());
        Assert.False(typeof(TestModelClass).HasCustomAttribute<AutoValidateNever>());
        Assert.True(typeof(TestModelRecord).HasCustomAttribute<AutoValidateNever>());
        Assert.False(typeof(TestModelRecord).HasCustomAttribute<AutoValidationAttribute>());
    }

    [AutoValidation]
    private class TestModelClass;

    [AutoValidateNever]
    private record TestModelRecord;

    private enum TestModelEnum;
}