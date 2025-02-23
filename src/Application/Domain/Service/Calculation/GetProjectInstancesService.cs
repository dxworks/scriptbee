using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Driven.Calculation;
using ScriptBee.Ports.Driving.UseCases.Calculation;

namespace ScriptBee.Domain.Service.Calculation;

public class GetProjectInstancesService(IGetAllProjectInstances getAllProjectInstances) : IGetProjectInstancesUseCase
{
    public async Task<IEnumerable<CalculationInstanceInfo>> GetAllInstances(ProjectId projectId,
        CancellationToken cancellationToken = default)
    {
        return await getAllProjectInstances.GetAll(projectId, cancellationToken);
    }
}
