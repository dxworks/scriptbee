namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python.Tests;

public class ScriptGeneratorStrategyUnitTests
{
    private readonly ScriptGeneratorStrategy _strategy = new();

    public enum LocalEnum { }

    public class LocalClass { }

    public class LocalChild : LocalClass { }

    [Fact]
    public void GenerateClassName_Enum_InheritsFromEnum()
    {
        var result = _strategy.GenerateClassName(typeof(LocalEnum));

        Assert.Contains("class LocalEnum(Enum):", result);
    }

    [Fact]
    public void GenerateClassName_Class_GeneratesClass()
    {
        var result = _strategy.GenerateClassName(typeof(LocalClass));

        Assert.Contains("class LocalClass:", result);
    }

    [Fact]
    public void GenerateClassName_EnumWithBaseClass_IgnoresBaseAndInheritsFromEnum()
    {
        var result = _strategy.GenerateClassName(typeof(LocalEnum), typeof(LocalClass), out _);

        Assert.Contains("class LocalEnum(Enum):", result);
    }

    [Fact]
    public void GenerateClassName_ClassWithBaseClass_InheritsFromBase()
    {
        var result = _strategy.GenerateClassName(typeof(LocalChild), typeof(LocalClass), out _);

        Assert.Contains("class LocalChild(LocalClass):", result);
    }

    [Fact]
    public void GenerateField_IntType_GeneratesTypedField()
    {
        var result = _strategy.GenerateField("public", typeof(int), "MyIntField", out _);

        Assert.Contains("MyIntField: int", result);
    }

    [Fact]
    public void GenerateField_StringType_GeneratesTypedField()
    {
        var result = _strategy.GenerateField("public", typeof(string), "MyStringField", out _);

        Assert.Contains("MyStringField: str", result);
    }

    [Fact]
    public void GenerateField_BoolType_GeneratesTypedField()
    {
        var result = _strategy.GenerateField("public", typeof(bool), "MyBoolField", out _);

        Assert.Contains("MyBoolField: bool", result);
    }

    [Fact]
    public void GenerateField_EnumType_GeneratesTypedField()
    {
        var result = _strategy.GenerateField("public", typeof(LocalEnum), "MyEnumField", out _);

        Assert.Contains("MyEnumField: LocalEnum", result);
    }

    [Fact]
    public void GenerateMethod_GeneratesDefWithPass()
    {
        var result = _strategy.GenerateMethod("public", typeof(void), "MyMethod", [], out _);

        Assert.Contains("def MyMethod():", result);
        Assert.Contains("pass", result);
    }

    [Fact]
    public void GenerateClassStart_ReturnsEmpty()
    {
        Assert.Equal("", _strategy.GenerateClassStart());
    }

    [Fact]
    public void GenerateClassEnd_ReturnsEmpty()
    {
        Assert.Equal("", _strategy.GenerateClassEnd());
    }

    [Fact]
    public void GenerateEmptyClass_ReturnsPass()
    {
        Assert.Equal("    pass", _strategy.GenerateEmptyClass());
    }

    [Fact]
    public void GenerateModelDeclaration_ContainsModelType()
    {
        var result = _strategy.GenerateModelDeclaration("MyModel");

        Assert.Contains("MyModel", result);
    }
}
