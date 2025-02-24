using ScriptBee.Domain.Model.Calculation;

namespace ScriptBee.Ports.Driven.Calculation;

public interface IDeallocateInstance
{
    Task Deallocate(CalculationInstanceInfo calculationInstanceInfo);
}
