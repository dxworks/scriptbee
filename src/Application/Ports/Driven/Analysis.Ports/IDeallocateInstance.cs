using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Analysis.Ports;

public interface IDeallocateInstance
{
    Task Deallocate(InstanceInfo calculationInstanceInfo);
}
