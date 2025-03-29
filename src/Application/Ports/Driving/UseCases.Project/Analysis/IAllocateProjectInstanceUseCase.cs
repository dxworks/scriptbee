using OneOf;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IAllocateProjectInstanceUseCase
{
    Task<OneOf<InstanceInfo, ProjectDoesNotExistsError>> Allocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
