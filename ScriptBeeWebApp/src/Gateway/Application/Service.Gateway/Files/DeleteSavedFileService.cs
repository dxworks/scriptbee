using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Files;

namespace ScriptBee.Service.Gateway.Files;

using DeleteResult = OneOf<Success, ProjectDoesNotExistsError, ProjectFileDoesNotExistsError>;

public class DeleteSavedFileService(
    IGetProject getProject,
    IFileModelService fileModelService,
    IUpdateProject updateProject
) : IDeleteSavedFileUseCase
{
    public async Task<DeleteResult> Delete(
        DeleteSavedFileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<DeleteResult>>(
            details => Delete(details, command, cancellationToken),
            error => Task.FromResult<DeleteResult>(error)
        );
    }

    private async Task<DeleteResult> Delete(
        ProjectDetails projectDetails,
        DeleteSavedFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var file = projectDetails.SavedFiles.FirstOrDefault(f => f.Id.Equals(command.FileId));
        if (file is null)
        {
            return new ProjectFileDoesNotExistsError(command.FileId);
        }

        projectDetails.SavedFiles.Remove(file);

        RemoveFileFromLoadedFiles(projectDetails, command.FileId);

        await fileModelService.DeleteFilesAsync([command.FileId], cancellationToken);

        await updateProject.Update(projectDetails, cancellationToken);

        return new Success();
    }

    private static void RemoveFileFromLoadedFiles(ProjectDetails projectDetails, FileId fileId)
    {
        foreach (var (loaderId, loadedFiles) in projectDetails.LoadedFiles)
        {
            loadedFiles.RemoveAll(f => f.Id.Equals(fileId));
        }
    }
}
