using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetCurrentInstanceUseCase
{
    Task<OneOf<InstanceInfo, NoInstanceAllocatedForProjectError>> GetCurrentInstance(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
