using ScriptBee.Domain.Model.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class CalculationInstanceService : ICalculationInstanceUseCase
{
    public Task<InstanceInfo> Allocate(
        CalculationInstanceCommand command,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task Deallocate(InstanceInfo instanceInfo)
    {
        throw new NotImplementedException();
    }
}
