using OneOf;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class AllocateProjectInstanceService : IAllocateProjectInstanceUseCase
{
    public Task<OneOf<InstanceInfo, ProjectDoesNotExistsError>> Allocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
