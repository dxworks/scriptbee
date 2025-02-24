using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Ports.Driving.UseCases.Calculation;

namespace ScriptBee.Domain.Service.Calculation;

public class CalculationInstanceService : ICalculationInstanceUseCase
{
    public Task<CalculationInstanceInfo> Allocate(CalculationInstanceCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Deallocate(CalculationInstanceInfo calculationInstanceInfo)
    {
        throw new NotImplementedException();
    }
}
