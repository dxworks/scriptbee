using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IInstallPlugin
{
    Task Install(
        InstanceInfo instanceInfo,
        string pluginId,
        string pluginVersion,
        CancellationToken cancellationToken = default
    );
}
