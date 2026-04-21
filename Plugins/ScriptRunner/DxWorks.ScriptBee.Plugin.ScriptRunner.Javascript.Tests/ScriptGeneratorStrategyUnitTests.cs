namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.Tests;

public class ScriptGeneratorStrategyUnitTests
{
    private readonly ScriptGeneratorStrategy _strategy = new();

    public enum LocalEnum { }

    public class LocalClass { }

    public class LocalChild : LocalClass { }

    [Fact]
    public void GenerateClassName_Enum_UsesClassKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalEnum));

        Assert.Contains("class LocalEnum", result);
    }

    [Fact]
    public void GenerateClassName_Class_UsesClassKeyword()
    {
        var result = _strategy.GenerateClassName(typeof(LocalClass));

        Assert.Contains("class LocalClass", result);
    }

    [Fact]
    public void GenerateClassName_WithBaseClass_IncludesExtends()
    {
        var result = _strategy.GenerateClassName(typeof(LocalChild), typeof(LocalClass), out _);

        Assert.Contains("class LocalChild extends LocalClass", result);
    }

    [Fact]
    public void GenerateField_EnumType_InitializesWithZero()
    {
        var result = _strategy.GenerateField("public", typeof(LocalEnum), "MyEnumField", out _);

        Assert.Contains("MyEnumField = 0;", result);
    }

    [Fact]
    public void GenerateField_IntType_InitializesWithZero()
    {
        var result = _strategy.GenerateField("public", typeof(int), "MyIntField", out _);

        Assert.Contains("MyIntField = 0;", result);
    }

    [Fact]
    public void GenerateField_StringType_InitializesWithEmptyString()
    {
        var result = _strategy.GenerateField("public", typeof(string), "MyStringField", out _);

        Assert.Contains("MyStringField = '';", result);
    }

    [Fact]
    public void GenerateField_BoolType_InitializesWithTrue()
    {
        var result = _strategy.GenerateField("public", typeof(bool), "MyBoolField", out _);

        Assert.Contains("MyBoolField = true;", result);
    }

    [Fact]
    public void GenerateMethod_EnumReturnType_ReturnsZero()
    {
        var result = _strategy.GenerateMethod(
            "public",
            typeof(LocalEnum),
            "MyEnumMethod",
            [],
            out _
        );

        Assert.Contains("return 0;", result);
    }

    [Fact]
    public void GenerateMethod_VoidReturnType_HasNoReturn()
    {
        var result = _strategy.GenerateMethod("public", typeof(void), "MyVoidMethod", [], out _);

        Assert.DoesNotContain("return", result);
    }

    [Fact]
    public void GenerateMethod_IntReturnType_ReturnsZero()
    {
        var result = _strategy.GenerateMethod("public", typeof(int), "MyIntMethod", [], out _);

        Assert.Contains("return 0;", result);
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
    public void GenerateEmptyClass_IsEmpty()
    {
        Assert.Equal("", _strategy.GenerateEmptyClass());
    }

    [Fact]
    public void GenerateModelDeclaration_ReturnsProjectInstantiation()
    {
        var result = _strategy.GenerateModelDeclaration("MyModel");

        Assert.Contains("new MyModel()", result);
    }
}
