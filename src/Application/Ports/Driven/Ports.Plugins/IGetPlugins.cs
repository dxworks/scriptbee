using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Ports.Plugins;

public interface IGetPlugins
{
    Task<IEnumerable<Plugin>> GetLoadedPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    );
}
