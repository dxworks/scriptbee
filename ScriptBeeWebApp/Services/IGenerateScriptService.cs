using DxWorks.ScriptBee.Plugin.Api;
using ScriptBeeWebApp.EndpointDefinitions.DTO;

namespace ScriptBeeWebApp.Services;

public interface IGenerateScriptService
{
    IEnumerable<ScriptLanguage> GetSupportedLanguages();
    IScriptGeneratorStrategy? GetGenerationStrategy(string scriptType);

    Task<Stream> GenerateClassesZip(IEnumerable<object> classes, IScriptGeneratorStrategy scriptGeneratorStrategy,
        CancellationToken cancellationToken = default);
}
