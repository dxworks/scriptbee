using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Calculation;

public interface IGetAllProjectInstances
{
    Task<IEnumerable<CalculationInstanceInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
