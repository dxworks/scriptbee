using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis.UseCases;

public interface IGetProjectInstancesUseCase
{
    Task<IEnumerable<InstanceInfo>> GetAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
