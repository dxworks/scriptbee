using OneOf;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Rest;

public class GetScriptLanguagesAdapter(IGetPlugins getPlugins) : IGetScriptLanguages
{
    public async Task<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>> Get(
        InstanceInfo instanceInfo,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        var loadedPlugins = await getPlugins.GetLoadedPlugins(instanceInfo, cancellationToken);

        var pluginExtensionPoint = loadedPlugins
            .SelectMany(plugin => plugin.Manifest.ExtensionPoints)
            .FirstOrDefault(point =>
                point is ScriptRunnerPluginExtensionPoint scriptRunnerPluginExtensionPoint
                && scriptRunnerPluginExtensionPoint.Language == name
            );

        if (pluginExtensionPoint == null)
        {
            return new ScriptLanguageDoesNotExistsError(name);
        }

        var scriptRunnerPluginExtensionPoint =
            (ScriptRunnerPluginExtensionPoint)pluginExtensionPoint;

        return new ScriptLanguage(
            scriptRunnerPluginExtensionPoint.Language,
            scriptRunnerPluginExtensionPoint.Extension
        );
    }
}
