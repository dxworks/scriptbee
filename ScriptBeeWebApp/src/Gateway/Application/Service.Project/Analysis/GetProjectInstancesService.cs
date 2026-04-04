using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetProjectInstancesService(
    IGetAllProjectInstances getAllProjectInstances,
    IGetInstanceStatus getInstanceStatus
) : IGetProjectInstancesUseCase
{
    public async Task<IEnumerable<InstanceInfo>> GetAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);
        var instanceInfos = instances.ToList();

        var tasks = instanceInfos.Select(async instance =>
        {
            var status = await getInstanceStatus.GetStatus(instance.Id, cancellationToken);
            return instance with { Status = status };
        });

        return await Task.WhenAll(tasks);
    }
}
