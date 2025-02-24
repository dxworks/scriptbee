using ScriptBee.Domain.Model.Calculation;

namespace ScriptBee.Ports.Driving.UseCases.Calculation;

public interface ICalculationInstanceUseCase
{
    Task<CalculationInstanceInfo> Allocate(CalculationInstanceCommand command,
        CancellationToken cancellationToken = default);

    Task Deallocate(CalculationInstanceInfo calculationInstanceInfo);
}
