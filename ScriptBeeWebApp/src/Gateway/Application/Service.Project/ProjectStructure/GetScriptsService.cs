using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public class GetScriptsService(IGetScripts getScripts) : IGetScriptsUseCase
{
    public async Task<IEnumerable<Script>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        return await getScripts.GetAll(projectId, cancellationToken);
    }

    public async Task<OneOf<Script, ScriptDoesNotExistsError>> GetById(
        ProjectId projectId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        return await getScripts.Get(scriptId, cancellationToken);
    }
}
