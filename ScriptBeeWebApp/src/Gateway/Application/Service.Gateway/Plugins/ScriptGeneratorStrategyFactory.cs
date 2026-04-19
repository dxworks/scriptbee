using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;
using OneOf;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.Service.Gateway.Plugins;

using CSharpStrategy = ScriptGeneratorStrategy;
using JavascriptStrategy = DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.ScriptGeneratorStrategy;
using PythonStrategy = DxWorks.ScriptBee.Plugin.ScriptRunner.Python.ScriptGeneratorStrategy;

public sealed class ScriptGeneratorStrategyFactory
{
    private static readonly IList<IScriptGeneratorStrategy> Strategies =
    [
        new CSharpStrategy(),
        new JavascriptStrategy(),
        new PythonStrategy(),
    ];

    public OneOf<IScriptGeneratorStrategy, UnknownScriptGeneratorStrategyError> Get(string language)
    {
        var strategy = Strategies.FirstOrDefault(s => s.Language == language);

        if (strategy == null)
        {
            return new UnknownScriptGeneratorStrategyError(language);
        }

        return OneOf<IScriptGeneratorStrategy, UnknownScriptGeneratorStrategyError>.FromT0(
            strategy
        );
    }
}
