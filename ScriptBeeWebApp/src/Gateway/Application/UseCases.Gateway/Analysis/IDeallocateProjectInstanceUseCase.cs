using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Analysis;

public interface IDeallocateProjectInstanceUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError>> Deallocate(
        ProjectId projectId,
        InstanceId instanceId,
        CancellationToken cancellationToken
    );
}
