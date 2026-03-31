using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public class GetScriptsService(IGetScripts getScripts, ILoadFile loadFile) : IGetScriptsUseCase
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

    public async Task<OneOf<string, ScriptDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        var result = await getScripts.Get(scriptId, cancellationToken);

        return await result.Match<Task<OneOf<string, ScriptDoesNotExistsError>>>(
            async script => await GetScriptContent(projectId, script, cancellationToken),
            error => Task.FromResult<OneOf<string, ScriptDoesNotExistsError>>(error)
        );
    }

    private async Task<OneOf<string, ScriptDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        Script script,
        CancellationToken cancellationToken
    )
    {
        var result = await loadFile.GetScriptContent(projectId, script.FilePath, cancellationToken);

        return await result.Match<Task<OneOf<string, ScriptDoesNotExistsError>>>(
            content => Task.FromResult<OneOf<string, ScriptDoesNotExistsError>>(content),
            _ =>
                Task.FromResult<OneOf<string, ScriptDoesNotExistsError>>(
                    new ScriptDoesNotExistsError(script.Id)
                )
        );
    }
}
