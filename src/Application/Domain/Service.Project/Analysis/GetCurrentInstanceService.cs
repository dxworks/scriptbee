using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetCurrentInstanceService : IGetCurrentInstanceUseCase
{
    public Task<InstanceInfo> GetOrAllocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#45): implement it
        throw new NotImplementedException();
    }
}
