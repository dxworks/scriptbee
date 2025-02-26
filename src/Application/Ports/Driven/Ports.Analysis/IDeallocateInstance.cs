using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Analysis;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo calculationInstanceInfo);
}
