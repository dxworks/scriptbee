using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface ILinkInstanceContext
{
    Task Link(
        InstanceInfo instanceInfo,
        IEnumerable<string> linkerIds,
        CancellationToken cancellationToken = default
    );
}
