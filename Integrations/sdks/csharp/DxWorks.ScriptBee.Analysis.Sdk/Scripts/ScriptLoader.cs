using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Scripts;

public sealed class ScriptLoader(ILoadFile loadFile, IGetScripts getScripts) : IScriptLoader
{
    public async Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        return await getScripts.Get(scriptId, cancellationToken);
    }

    public async Task<OneOf<string, ScriptDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken
    )
    {
        var result = await loadFile.GetScriptContent(projectId, path, cancellationToken);

        return result.Match<OneOf<string, ScriptDoesNotExistsError>>(
            content => content,
            error => new ScriptDoesNotExistsError(error.Path)
        );
    }
}
