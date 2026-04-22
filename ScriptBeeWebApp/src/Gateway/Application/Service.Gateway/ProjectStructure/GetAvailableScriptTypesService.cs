using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Gateway.ProjectStructure;

namespace ScriptBee.Service.Gateway.ProjectStructure;

public class GetAvailableScriptTypesService(IPluginRepository pluginRepository)
    : IGetAvailableScriptTypesUseCase
{
    public IEnumerable<ScriptLanguage> GetAvailableScriptTypes()
    {
        var defaultLanguages = new List<ScriptLanguage>
        {
            new("csharp", ".cs"),
            new("python", ".py"),
            new("javascript", ".js"),
        };

        var pluginLanguages = pluginRepository
            .GetLoadedPlugins()
            .SelectMany(p => p.Manifest.ExtensionPoints)
            .OfType<ScriptRunnerPluginExtensionPoint>()
            .Where(r =>
                !defaultLanguages.Any(dl =>
                    dl.Name.Equals(r.Language, StringComparison.InvariantCultureIgnoreCase)
                )
            )
            .Select(r => new ScriptLanguage(r.Language, r.Extension));

        return defaultLanguages.Concat(pluginLanguages);
    }
}
