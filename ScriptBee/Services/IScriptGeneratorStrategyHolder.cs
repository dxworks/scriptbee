using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;

namespace ScriptBee.Services;

public interface IScriptGeneratorStrategyHolder
{
    public void AddStrategy(IScriptGeneratorStrategy scriptGeneratorStrategy);

    public IScriptGeneratorStrategy? GetStrategy(string language);

    public IEnumerable<IScriptGeneratorStrategy> GetAllStrategies();
}
