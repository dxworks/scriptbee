using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using UpdateResult = OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError>;

public class UpdateScriptService(
    IGetProject getProject,
    IGetScripts getScripts,
    IUpdateScript updateScript
) : IUpdateScriptUseCase
{
    public async Task<UpdateResult> Update(
        UpdateScriptCommand command,
        CancellationToken cancellationToken
    )
    {
        var projectDetailsResult = await getProject.GetById(command.ProjectId, cancellationToken);

        return await projectDetailsResult.Match<Task<UpdateResult>>(
            async _ => await UpdateScript(command, cancellationToken),
            error => Task.FromResult<UpdateResult>(error)
        );
    }

    private async Task<UpdateResult> UpdateScript(
        UpdateScriptCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getScripts.Get(command.ScriptId, cancellationToken);

        return await result.Match<Task<UpdateResult>>(
            async script => await Update(command, script, cancellationToken),
            error => Task.FromResult<UpdateResult>(error)
        );
    }

    private async Task<UpdateResult> Update(
        UpdateScriptCommand command,
        Script script,
        CancellationToken cancellationToken
    )
    {
        if (command.Parameters == null)
        {
            return script;
        }

        var updatedScript = script with { Parameters = command.Parameters };

        return await updateScript.Update(updatedScript, cancellationToken);
    }
}
