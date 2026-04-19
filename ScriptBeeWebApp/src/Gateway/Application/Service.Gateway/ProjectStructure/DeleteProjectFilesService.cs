using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Notifications;
using ScriptBee.Ports.Notifications.Events;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.ProjectStructure;

namespace ScriptBee.Service.Gateway.ProjectStructure;

using DeleteResult = OneOf<Success, ProjectDoesNotExistsError>;

public sealed class DeleteProjectFilesService(
    IGetProject getProject,
    IDeleteScript deleteScript,
    IDeleteFileOrFolder deleteFileOrFolder,
    IProjectNotificationsService projectNotificationsService
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

            await projectNotificationsService.NotifyScriptDeleted(
                new ScriptDeletedEvent(command.ProjectId, command.Id),
                cancellationToken
            );
        }

        return new Success();
    }
}
