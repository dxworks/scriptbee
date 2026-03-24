using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Analysis.Integration;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo calculationInstanceInfo);
}
