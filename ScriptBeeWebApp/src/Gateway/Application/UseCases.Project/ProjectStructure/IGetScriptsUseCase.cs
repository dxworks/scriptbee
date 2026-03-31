using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IGetScriptsUseCase
{
    Task<IEnumerable<Script>> GetAll(ProjectId projectId, CancellationToken cancellationToken);
}
