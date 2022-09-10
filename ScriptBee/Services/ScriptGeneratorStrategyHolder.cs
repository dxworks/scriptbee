using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using Serilog;

namespace ScriptBee.Services;

public class ScriptGeneratorStrategyHolder : IScriptGeneratorStrategyHolder
{
    private readonly Dictionary<string, IScriptGeneratorStrategy> _strategies = new();

    private readonly ILogger _logger;

    public ScriptGeneratorStrategyHolder(ILogger logger)
    {
        _logger = logger;
    }

    public void AddStrategy(IScriptGeneratorStrategy scriptGeneratorStrategy)
    {
        var language = scriptGeneratorStrategy.Language.ToLower();

        if (!_strategies.ContainsKey(language))
        {
            _strategies.Add(language, scriptGeneratorStrategy);
        }
        else
        {
            _logger.Warning("Script generator strategy for language {language} already exists", language);
        }
    }

    public IScriptGeneratorStrategy? GetStrategy(string language)
    {
        return _strategies.TryGetValue(language, out var loader) ? loader : null;
    }

    public IEnumerable<IScriptGeneratorStrategy> GetAllStrategies()
    {
        return _strategies.Select(pair => pair.Value);
    }
}
