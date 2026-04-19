using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Analysis;

public interface IGetProjectInstancesUseCase
{
    Task<IEnumerable<InstanceInfo>> GetAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken
    );

    Task<OneOf<InstanceInfo, InstanceDoesNotExistsError>> GetInstance(
        ProjectId projectId,
        InstanceId instanceId,
        CancellationToken cancellationToken
    );
}
