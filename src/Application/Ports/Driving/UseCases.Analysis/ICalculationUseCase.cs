using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Analysis;

public interface ICalculationUseCase
{
    Task<InstanceInfo> Run(ProjectId projectId, CancellationToken cancellationToken = default);
}
