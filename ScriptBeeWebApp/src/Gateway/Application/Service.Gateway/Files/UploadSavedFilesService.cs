using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Files;

namespace ScriptBee.Service.Gateway.Files;

using UploadResult = OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>;

public class UploadSavedFilesService(
    IGetProject getProject,
    IFileModelService fileModelService,
    IGuidProvider guidProvider,
    IUpdateProject updateProject
) : IUploadSavedFilesUseCase
{
    public async Task<UploadResult> Upload(
        UploadSavedFilesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UploadResult>>(
            async details => await Upload(details, command.UploadFiles, cancellationToken),
            error => Task.FromResult<UploadResult>(error)
        );
    }

    private async Task<UploadResult> Upload(
        ProjectDetails projectDetails,
        IEnumerable<UploadFileInformation> uploadFiles,
        CancellationToken cancellationToken
    )
    {
        var savedFilesData = await UploadFiles(uploadFiles, cancellationToken);

        projectDetails.SavedFiles.AddRange(savedFilesData);

        await updateProject.Update(projectDetails, cancellationToken);

        return savedFilesData;
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
            await fileModelService.UploadFileAsync<object>(
                fileData.Id,
                stream,
                null,
                cancellationToken
            );

            savedFilesData.Add(fileData);
        }

        return savedFilesData;
    }
}
