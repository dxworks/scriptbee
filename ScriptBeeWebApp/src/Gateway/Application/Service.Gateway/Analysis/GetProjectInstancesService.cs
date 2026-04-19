using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetProjectInstancesService(
    IGetAllProjectInstances getAllProjectInstances,
    IGetProjectInstance getProjectInstance,
    IGetInstanceStatus getInstanceStatus
) : IGetProjectInstancesUseCase
{
    public async Task<IEnumerable<InstanceInfo>> GetAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken
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

    public async Task<OneOf<InstanceInfo, InstanceDoesNotExistsError>> GetInstance(
        ProjectId projectId,
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<OneOf<InstanceInfo, InstanceDoesNotExistsError>>>(
            async info =>
            {
                var status = await getInstanceStatus.GetStatus(instanceId, cancellationToken);

                return info with
                {
                    Status = status,
                };
            },
            error => Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(error)
        );
    }
}
