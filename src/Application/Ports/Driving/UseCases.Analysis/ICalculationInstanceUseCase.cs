using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.UseCases.Analysis;

public interface ICalculationInstanceUseCase
{
    Task<InstanceInfo> Allocate(
        CalculationInstanceCommand command,
        CancellationToken cancellationToken = default
    );

    Task Deallocate(InstanceInfo instanceInfo);
}
