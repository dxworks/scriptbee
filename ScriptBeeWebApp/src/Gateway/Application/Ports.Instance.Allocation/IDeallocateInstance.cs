using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance.Allocation;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo instanceInfo, CancellationToken cancellationToken);
}
