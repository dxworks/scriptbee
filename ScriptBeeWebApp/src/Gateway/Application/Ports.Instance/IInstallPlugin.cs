using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Ports.Instance;

public interface IInstallPlugin
{
    Task Install(InstanceInfo instanceInfo, PluginId pluginId, CancellationToken cancellationToken);
}
