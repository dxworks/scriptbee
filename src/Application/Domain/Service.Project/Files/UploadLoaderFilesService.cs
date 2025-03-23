using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Files;

namespace ScriptBee.Service.Project.Files;

using UploadResult = OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>;

public class UploadLoaderFilesService(
    IGetProject getProject,
    IFileModelService fileModelService,
    IGuidProvider guidProvider,
    IUpdateProject updateProject
) : IUploadLoaderFilesUseCase
{
    public async Task<UploadResult> Upload(
        UploadLoaderFilesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UploadResult>>(
            async details =>
                await Upload(details, command.LoaderId, command.UploadFiles, cancellationToken),
            error => Task.FromResult<UploadResult>(error)
        );
    }

    private async Task<UploadResult> Upload(
        ProjectDetails projectDetails,
        string loaderId,
        IEnumerable<UploadFileInformation> uploadFiles,
        CancellationToken cancellationToken = default
    )
    {
        await DeletePreviousSavedFiles(projectDetails, loaderId, cancellationToken);

        var savedFilesData = await UploadFiles(uploadFiles, cancellationToken);

        projectDetails.SavedFiles[loaderId] = savedFilesData;

        await updateProject.Update(projectDetails, cancellationToken);

        return savedFilesData;
    }

    private async Task DeletePreviousSavedFiles(
        ProjectDetails projectDetails,
        string loaderId,
        CancellationToken cancellationToken
    )
    {
        var filesToDelete = new List<string>();
        if (projectDetails.SavedFiles.TryGetValue(loaderId, out var previousSavedFilesData))
        {
            filesToDelete.AddRange(previousSavedFilesData.Select(data => data.Id.ToString()));
        }

        await fileModelService.DeleteFilesAsync(filesToDelete, cancellationToken);
    }

    private async Task<List<FileData>> UploadFiles(
        IEnumerable<UploadFileInformation> files,
        CancellationToken cancellationToken
    )
    {
        var savedFilesData = new List<FileData>();

        foreach (var file in files.Where(f => f.Length > 0))
        {
            var fileData = new FileData(new FileId(guidProvider.NewGuid()), file.FileName);

            await using var stream = file.FileStream;
            await fileModelService.UploadFileAsync(
                fileData.Id.ToString(),
                stream,
                cancellationToken
            );

            savedFilesData.Add(fileData);
        }

        return savedFilesData;
    }
}
