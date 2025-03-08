using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Instance;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo calculationInstanceInfo);
}
