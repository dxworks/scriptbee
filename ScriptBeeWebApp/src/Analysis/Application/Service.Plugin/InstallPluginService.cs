using OneOf;
using OneOf.Types;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Plugin;
using ScriptBee.UseCases.Plugin.Errors;

namespace ScriptBee.Service.Plugin;

public class InstallPluginService(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    IPluginPathProvider pluginPathProvider
) : IInstallPluginUseCase
{
    public OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(
        string pluginId,
        string version
    )
    {
        try
        {
            var plugin = pluginReader.ReadPlugin(
                pluginPathProvider.GetPathToPlugins(),
                pluginId,
                version
            );

            if (plugin is null)
            {
                return new InvalidPluginError(pluginId, version);
            }

            pluginLoader.Load(plugin);
            return new Success();
        }
        catch
        {
            return new PluginInstallationError(pluginId, version);
        }
    }
}
