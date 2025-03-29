using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface ICreateProjectInstance
{
    Task<InstanceInfo> Create(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    );
}
