using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetCurrentInstanceService(IGetAllProjectInstances getAllProjectInstances)
    : IGetCurrentInstanceUseCase
{
    public async Task<OneOf<InstanceInfo, NoInstanceAllocatedForProjectError>> GetCurrentInstance(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        var instanceInfos = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        var instanceInfo = instanceInfos.FirstOrDefault();

        if (instanceInfo == null)
        {
            return new NoInstanceAllocatedForProjectError(projectId);
        }

        return instanceInfo;
    }
}
