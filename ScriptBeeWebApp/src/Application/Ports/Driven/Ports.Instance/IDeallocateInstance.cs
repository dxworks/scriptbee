using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo calculationInstanceInfo);
}
