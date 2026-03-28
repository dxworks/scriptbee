using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IUninstallPlugin
{
    Task Uninstall(
        InstanceInfo instanceInfo,
        string pluginId,
        string pluginVersion,
        CancellationToken cancellationToken = default
    );
}
