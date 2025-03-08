using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

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
