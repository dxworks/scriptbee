using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using UpdateContentResult = OneOf<Success, ProjectDoesNotExistsError, ScriptDoesNotExistsError>;
using UpdateResult = OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError>;

public class UpdateScriptService(
    IGetProject getProject,
    IGetScripts getScripts,
    IUpdateScript updateScript,
    IUpdateFile updateFile
) : IUpdateScriptUseCase
{
    public async Task<UpdateResult> Update(
        UpdateScriptCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UpdateResult>>(
            async _ => await UpdateScript(command, cancellationToken),
            error => Task.FromResult<UpdateResult>(error)
        );
    }

    public async Task<UpdateContentResult> UpdateContent(
        UpdateScriptContentCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UpdateContentResult>>(
            async _ => await UpdateScriptContent(command, cancellationToken),
            error => Task.FromResult<UpdateContentResult>(error)
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
        var updatedScript = script;

        if (command.Parameters is not null)
        {
            updatedScript = updatedScript with { Parameters = command.Parameters };
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            updatedScript = updatedScript with
            {
                File = updatedScript.File.UpdateName(command.Name),
            };
        }

        if (updatedScript == script)
        {
            return script;
        }

        if (script.File != updatedScript.File)
        {
            updateFile.RenameFile(command.ProjectId, script.File.Path, updatedScript.File.Path);
        }

        return await updateScript.Update(updatedScript, cancellationToken);
    }

    private async Task<UpdateContentResult> UpdateScriptContent(
        UpdateScriptContentCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getScripts.Get(command.ScriptId, cancellationToken);

        return await result.Match<Task<UpdateContentResult>>(
            async script =>
            {
                await updateFile.UpdateContent(
                    command.ProjectId,
                    script.File.Path,
                    command.Content,
                    cancellationToken
                );
                return new Success();
            },
            error => Task.FromResult<UpdateContentResult>(error)
        );
    }
}
