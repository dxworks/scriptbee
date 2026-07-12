using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Gateway.Plugins;

public interface IManagePluginsUseCase
{
    void LoadPlugins();

    IEnumerable<Plugin> GetInstalledPlugins();

    Dictionary<string, string> GetUiPluginsManifest();

    Task Install(PluginId pluginId, CancellationToken cancellationToken);

    void Uninstall(PluginId pluginId);

    string? GetUiPluginFilePath(PluginId pluginId, string filePath);
}
