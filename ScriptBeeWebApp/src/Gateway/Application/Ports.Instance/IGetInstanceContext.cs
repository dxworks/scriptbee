using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IGetInstanceContext
{
    Task<IEnumerable<ContextSlice>> Get(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    );
}
