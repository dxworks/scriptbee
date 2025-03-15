using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetCurrentInstanceUseCase
{
    Task<InstanceInfo> GetOrAllocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
