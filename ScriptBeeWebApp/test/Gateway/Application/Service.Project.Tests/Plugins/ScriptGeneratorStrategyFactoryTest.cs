using ScriptBee.Domain.Model.Errors;
using ScriptBee.Service.Project.Plugins;
using CSharpStrategy = DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.ScriptGeneratorStrategy;
using JavascriptStrategy = DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.ScriptGeneratorStrategy;
using PythonStrategy = DxWorks.ScriptBee.Plugin.ScriptRunner.Python.ScriptGeneratorStrategy;

namespace ScriptBee.Service.Project.Tests.Plugins;

public class ScriptGeneratorStrategyFactoryTest
{
    private readonly ScriptGeneratorStrategyFactory _factory = new();

    [Fact]
    public void GivenCSharp_ShouldReturnStrategy()
    {
        var result = _factory.Get("csharp");

        result.AsT0.ShouldBeOfType<CSharpStrategy>();
    }

    [Fact]
    public void GivenJavascript_ShouldReturnStrategy()
    {
        var result = _factory.Get("javascript");

        result.AsT0.ShouldBeOfType<JavascriptStrategy>();
    }

    [Fact]
    public void GivenPython_ShouldReturnStrategy()
    {
        var result = _factory.Get("python");

        result.AsT0.ShouldBeOfType<PythonStrategy>();
    }

    [Fact]
    public void GivenUnknownLanguage_ShouldReturnUnknownScriptGeneratorStrategyError()
    {
        var result = _factory.Get("unknown");

        result.AsT1.ShouldBe(new UnknownScriptGeneratorStrategyError("unknown"));
    }
}
