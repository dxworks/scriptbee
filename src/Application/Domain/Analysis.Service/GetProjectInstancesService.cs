using ScriptBee.Analysis.Ports;
using ScriptBee.Analysis.UseCases;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis.Service;

public class GetProjectInstancesService(IGetAllProjectInstances getAllProjectInstances)
    : IGetProjectInstancesUseCase
{
    public async Task<IEnumerable<InstanceInfo>> GetAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAllProjectInstances.GetAll(projectId, cancellationToken);
    }
}
