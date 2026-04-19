using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Ports.Instance;

public interface IGetPlugins
{
    Task<IEnumerable<Plugin>> GetLoadedPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    );
}
