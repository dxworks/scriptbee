using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Gateway.Plugins;

public interface IManagePluginsUseCase
{
    void LoadPlugins();

    IEnumerable<PluginId> GetInstalledPlugins();

    Task Install(PluginId pluginId, CancellationToken cancellationToken);

    void Uninstall(PluginId pluginId);
}
