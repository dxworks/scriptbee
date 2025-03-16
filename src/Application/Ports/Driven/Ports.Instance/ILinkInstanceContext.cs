using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Instance;

public interface ILinkInstanceContext
{
    Task Link(
        InstanceInfo instanceInfo,
        IEnumerable<string> linkerIds,
        CancellationToken cancellationToken = default
    );
}
