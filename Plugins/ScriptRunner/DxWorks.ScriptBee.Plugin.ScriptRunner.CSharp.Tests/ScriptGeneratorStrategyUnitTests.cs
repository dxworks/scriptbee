using System.Reflection;
using System.Reflection.Emit;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

public class ScriptGeneratorStrategyUnitTests
{
    private readonly ScriptGeneratorStrategy _strategy = new();

    public enum LocalEnum { }

    public struct LocalStruct { }

    public class LocalClass { }

    public class LocalChild : LocalClass { }

    [Fact]
    public void GenerateClassName_Enum_UsesEnumKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalEnum));

        Assert.Contains("public enum LocalEnum", result);
    }

    [Fact]
    public void GenerateClassName_Struct_UsesStructKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalStruct));

        Assert.Contains("public struct LocalStruct", result);
    }

    [Fact]
    public void GenerateClassName_Class_UsesClassKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalClass));

        Assert.Contains("public class LocalClass", result);
    }

    [Fact]
    public void GenerateClassName_TypeWithNamespace_IncludesFileScopedNamespace()
    {
        var result = _strategy.GenerateClassName(typeof(LocalClass));

        Assert.StartsWith(
            $"namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;{Environment.NewLine}{Environment.NewLine}",
            result
        );
    }

    [Fact]
    public void GenerateClassName_WithBaseClass_IncludesInheritance()
    {
        var result = _strategy.GenerateClassName(typeof(LocalChild), typeof(LocalClass), out _);

        Assert.Contains("public class LocalChild : LocalClass", result);
    }

    [Fact]
    public void GenerateClassName_EnumWithBaseClass_UsesEnumKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalEnum), typeof(LocalClass), out _);

        Assert.Contains("public enum LocalEnum", result);
    }

    [Fact]
    public void GenerateClassName_WithBaseClass_IncludesNamespace()
    {
        var result = _strategy.GenerateClassName(typeof(LocalChild), typeof(LocalClass), out _);

        Assert.StartsWith(
            $"namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;{Environment.NewLine}{Environment.NewLine}",
            result
        );
    }

    [Fact]
    public void GenerateField_WithEnumType_GeneratesCorrectTypeName()
    {
        var result = _strategy.GenerateField(
            "public",
            typeof(LocalEnum),
            "MyField",
            out var genericTypes
        );

        Assert.Contains("LocalEnum MyField", result);
        Assert.Empty(genericTypes);
    }

    [Fact]
    public void GenerateProperty_WithEnumType_GeneratesCorrectTypeName()
    {
        var result = _strategy.GenerateProperty(
            "public",
            typeof(LocalEnum),
            "MyProp",
            out var genericTypes
        );

        Assert.Contains("LocalEnum MyProp", result);
        Assert.Empty(genericTypes);
    }

    [Fact]
    public void GenerateMethod_WithEnumReturnType_GeneratesCorrectTypeName()
    {
        var result = _strategy.GenerateMethod("public", typeof(LocalEnum), "MyMethod", [], out _);

        Assert.Contains("LocalEnum MyMethod()", result);
        Assert.Contains("return default;", result);
    }

    [Fact]
    public void GenerateClassStart_ReturnsOpenBrace()
    {
        Assert.Equal("{", _strategy.GenerateClassStart());
    }

    [Fact]
    public void GenerateClassEnd_ReturnsCloseBrace()
    {
        Assert.Equal("}", _strategy.GenerateClassEnd());
    }

    [Fact]
    public void GenerateEmptyClass_ReturnsEmpty()
    {
        Assert.Equal("", _strategy.GenerateEmptyClass());
    }

    [Fact]
    public void GenerateModelDeclaration_ReturnsEmpty()
    {
        Assert.Equal("", _strategy.GenerateModelDeclaration("SomeModel"));
    }

    [Fact]
    public void GenerateClassName_TypeWithNoNamespace_DoesNotIncludeNamespaceDeclaration()
    {
        var typeWithoutNamespace = CreateTypeWithoutNamespace(
            "NoNamespaceClass",
            TypeAttributes.Public | TypeAttributes.Class
        );

        var result = _strategy.GenerateClassName(typeWithoutNamespace);

        Assert.DoesNotContain("namespace", result);
        Assert.Contains("public class NoNamespaceClass", result);
    }

    [Fact]
    public void GenerateClassName_TypeWithNoNamespace_WithBaseClass_DoesNotIncludeNamespaceDeclaration()
    {
        var typeWithoutNamespace = CreateTypeWithoutNamespace(
            "NoNamespaceChild",
            TypeAttributes.Public | TypeAttributes.Class
        );

        var result = _strategy.GenerateClassName(typeWithoutNamespace, typeof(LocalClass), out _);

        Assert.DoesNotContain("namespace", result);
        Assert.Contains("public class NoNamespaceChild", result);
    }

    private static Type CreateTypeWithoutNamespace(string typeName, TypeAttributes typeAttributes)
    {
        var assemblyName = new AssemblyName($"DynamicAssembly_{typeName}");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.Run
        );
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes);
        return typeBuilder.CreateType()!;
    }
}
