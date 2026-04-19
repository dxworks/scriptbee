using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.Service.Analysis;

public class InstallPluginService(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    IPluginPathProvider pluginPathProvider
) : IInstallPluginUseCase
{
    public OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(
        PluginId pluginId
    )
    {
        try
        {
            var plugin = pluginReader.ReadPlugin(pluginPathProvider.GetPathToPlugins(), pluginId);

            if (plugin is null)
            {
                return new InvalidPluginError(pluginId);
            }

            pluginLoader.Load(plugin);
            return new Success();
        }
        catch
        {
            return new PluginInstallationError(pluginId);
        }
    }
}
