using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using DeleteResult = OneOf<Success, ProjectDoesNotExistsError>;

public sealed class DeleteProjectFilesService(
    IGetProject getProject,
    IDeleteScript deleteScript,
    IDeleteFileOrFolder deleteFileOrFolder
) : IDeleteProjectFilesUseCase
{
    public async Task<DeleteResult> Delete(
        DeleteFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<DeleteResult>>(
            async _ => await DeleteScriptEntryAndFileOrFolder(command, cancellationToken),
            error => Task.FromResult<DeleteResult>(error)
        );
    }

    private async Task<DeleteResult> DeleteScriptEntryAndFileOrFolder(
        DeleteFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var projectStructureEntry = await deleteScript.Delete(command.Id, cancellationToken);

        if (projectStructureEntry is not null)
        {
            deleteFileOrFolder.Delete(command.ProjectId, projectStructureEntry.File.Path);
        }

        return new Success();
    }
}
