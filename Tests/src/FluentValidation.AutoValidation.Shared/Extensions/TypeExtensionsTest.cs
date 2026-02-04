// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
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
        Assert.True(typeof(TestModelStruct).IsCustomType());
        Assert.False(typeof(TestModelEnum).IsCustomType());
        Assert.False(typeof(Enum).IsCustomType());
        Assert.False(typeof(string).IsCustomType());
        Assert.False(typeof(char).IsCustomType());
        Assert.False(typeof(short).IsCustomType());
        Assert.False(typeof(ushort).IsCustomType());
        Assert.False(typeof(int).IsCustomType());
        Assert.False(typeof(uint).IsCustomType());
        Assert.False(typeof(nint).IsCustomType());
        Assert.False(typeof(nuint).IsCustomType());
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
        Assert.False(typeof(Uri).IsCustomType());
    }

    [Fact]
    public void Test_IsCustomType_Collections()
    {
        Assert.True(typeof(ICollection<TestModelClass>).IsCustomType());
        Assert.True(typeof(ICollection<TestModelRecord>).IsCustomType());
        Assert.True(typeof(ICollection<TestModelStruct>).IsCustomType());
        Assert.False(typeof(ICollection<TestModelEnum>).IsCustomType());
        Assert.False(typeof(ICollection<Enum>).IsCustomType());
        Assert.False(typeof(ICollection<string>).IsCustomType());

        Assert.True(typeof(IList<TestModelClass>).IsCustomType());
        Assert.True(typeof(IList<TestModelRecord>).IsCustomType());
        Assert.True(typeof(IList<TestModelStruct>).IsCustomType());
        Assert.False(typeof(IList<TestModelEnum>).IsCustomType());
        Assert.False(typeof(IList<Enum>).IsCustomType());
        Assert.False(typeof(IList<string>).IsCustomType());

        Assert.True(typeof(List<TestModelClass>).IsCustomType());
        Assert.True(typeof(List<TestModelRecord>).IsCustomType());
        Assert.True(typeof(List<TestModelStruct>).IsCustomType());
        Assert.False(typeof(List<TestModelEnum>).IsCustomType());
        Assert.False(typeof(List<Enum>).IsCustomType());
        Assert.False(typeof(List<string>).IsCustomType());

        Assert.True(typeof(TestModelClass[]).IsCustomType());
        Assert.True(typeof(TestModelRecord[]).IsCustomType());
        Assert.True(typeof(TestModelStruct[]).IsCustomType());
        Assert.False(typeof(TestModelEnum[]).IsCustomType());
        Assert.False(typeof(Enum[]).IsCustomType());
        Assert.False(typeof(string[]).IsCustomType());

        Assert.True(typeof(Dictionary<string, TestModelClass>).IsCustomType());
        Assert.True(typeof(Dictionary<string, TestModelRecord>).IsCustomType());
        Assert.True(typeof(Dictionary<string, TestModelStruct>).IsCustomType());
        Assert.False(typeof(Dictionary<string, TestModelEnum>).IsCustomType());
        Assert.False(typeof(Dictionary<string, Enum>).IsCustomType());
        Assert.False(typeof(Dictionary<string, string>).IsCustomType());

        Assert.True(typeof(HashSet<TestModelClass>).IsCustomType());
        Assert.True(typeof(HashSet<TestModelRecord>).IsCustomType());
        Assert.True(typeof(HashSet<TestModelStruct>).IsCustomType());
        Assert.False(typeof(HashSet<TestModelEnum>).IsCustomType());
        Assert.False(typeof(HashSet<Enum>).IsCustomType());
        Assert.False(typeof(HashSet<string>).IsCustomType());

        Assert.True(typeof(IEnumerable<TestModelClass>).IsCustomType());
        Assert.True(typeof(IEnumerable<TestModelRecord>).IsCustomType());
        Assert.True(typeof(IEnumerable<TestModelStruct>).IsCustomType());
        Assert.False(typeof(IEnumerable<TestModelEnum>).IsCustomType());
        Assert.False(typeof(IEnumerable<Enum>).IsCustomType());
        Assert.False(typeof(IEnumerable<string>).IsCustomType());

        Assert.True(typeof(IReadOnlyList<TestModelClass>).IsCustomType());
        Assert.True(typeof(IReadOnlyList<TestModelRecord>).IsCustomType());
        Assert.True(typeof(IReadOnlyList<TestModelStruct>).IsCustomType());
        Assert.False(typeof(IReadOnlyList<TestModelEnum>).IsCustomType());
        Assert.False(typeof(IReadOnlyList<Enum>).IsCustomType());
        Assert.False(typeof(IReadOnlyList<string>).IsCustomType());

        Assert.True(typeof(IReadOnlyCollection<TestModelClass>).IsCustomType());
        Assert.True(typeof(IReadOnlyCollection<TestModelRecord>).IsCustomType());
        Assert.True(typeof(IReadOnlyCollection<TestModelStruct>).IsCustomType());
        Assert.False(typeof(IReadOnlyCollection<TestModelEnum>).IsCustomType());
        Assert.False(typeof(IReadOnlyCollection<Enum>).IsCustomType());
        Assert.False(typeof(IReadOnlyCollection<string>).IsCustomType());

        Assert.True(typeof(ISet<TestModelClass>).IsCustomType());
        Assert.True(typeof(ISet<TestModelRecord>).IsCustomType());
        Assert.True(typeof(ISet<TestModelStruct>).IsCustomType());
        Assert.False(typeof(ISet<TestModelEnum>).IsCustomType());
        Assert.False(typeof(ISet<Enum>).IsCustomType());
        Assert.False(typeof(ISet<string>).IsCustomType());

        Assert.False(typeof(IAsyncEnumerable<TestModelClass>).IsCustomType());
        Assert.False(typeof(IAsyncEnumerable<TestModelRecord>).IsCustomType());
        Assert.False(typeof(IAsyncEnumerable<TestModelStruct>).IsCustomType());
        Assert.False(typeof(IAsyncEnumerable<TestModelEnum>).IsCustomType());
        Assert.False(typeof(IAsyncEnumerable<Enum>).IsCustomType());
        Assert.False(typeof(IAsyncEnumerable<string>).IsCustomType());
    }

    [Fact]
    public void Test_HasCustomAttribute()
    {
        Assert.True(typeof(TestModelClass).HasCustomAttribute<AutoValidationAttribute>());
        Assert.False(typeof(TestModelClass).HasCustomAttribute<AutoValidateNeverAttribute>());
        Assert.True(typeof(TestModelRecord).HasCustomAttribute<AutoValidateNeverAttribute>());
        Assert.False(typeof(TestModelRecord).HasCustomAttribute<AutoValidationAttribute>());
    }

    [Fact]
    public void Test_InheritsFromTypeWithNameEndingIn()
    {
        Assert.True(typeof(TestInherits1).InheritsFromTypeWithNameEndingIn("Controller"));
        Assert.True(typeof(TestInherits1).InheritsFromTypeWithNameEndingIn("controller"));
        Assert.True(typeof(TestInherits2).InheritsFromTypeWithNameEndingIn("Controller"));
        Assert.True(typeof(TestInherits2).InheritsFromTypeWithNameEndingIn("controller"));
        Assert.False(typeof(TestInherits3).InheritsFromTypeWithNameEndingIn("Controller"));
        Assert.False(typeof(TestInherits3).InheritsFromTypeWithNameEndingIn("controller"));
        Assert.False(typeof(TestInherits4).InheritsFromTypeWithNameEndingIn("Controller"));
        Assert.False(typeof(TestInherits4).InheritsFromTypeWithNameEndingIn("controller"));
        Assert.False(typeof(TestInherits5).InheritsFromTypeWithNameEndingIn("Controller"));
        Assert.False(typeof(TestInherits5).InheritsFromTypeWithNameEndingIn("controller"));
    }

    [AutoValidation]
    private class TestModelClass;

    [AutoValidateNever]
    private record TestModelRecord;

    private struct TestModelStruct;

    private enum TestModelEnum;

    private class TestInherits1 : Controller;

    private class TestInherits2 : CustomControllerBase;

    private class TestInherits3 : ControllerBase;

    private class TestInherits4 : ActionContext;

    private class TestInherits5 : object;

    private class CustomControllerBase : Controller;
}