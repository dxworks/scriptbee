using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driving.UseCases.Calculation;

public interface IGetProjectInstancesUseCase
{
    Task<IEnumerable<CalculationInstanceInfo>> GetAllInstances(ProjectId projectId,
        CancellationToken cancellationToken = default);
}
