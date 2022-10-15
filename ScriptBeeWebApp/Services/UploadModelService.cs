using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ScriptBee.Models;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class UploadModelService : IUploadModelService
{
    private readonly IFileModelService _fileModelService;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IProjectModelService _projectModelService;

    public UploadModelService(IFileModelService fileModelService, IProjectModelService projectModelService,
        IGuidGenerator guidGenerator)
    {
        _fileModelService = fileModelService;
        _projectModelService = projectModelService;
        _guidGenerator = guidGenerator;
    }

    public async Task<List<FileData>> UploadFilesAsync(ProjectModel projectModel, string loaderName,
        IFormFileCollection files, CancellationToken cancellationToken = default)
    {
        await DeletePreviousSavedFiles(projectModel, loaderName, cancellationToken);

        var savedFilesData = await UploadFiles(files, cancellationToken);

        projectModel.SavedFiles[loaderName] = savedFilesData;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return savedFilesData;
    }

    private async Task<List<FileData>> UploadFiles(IFormFileCollection files, CancellationToken cancellationToken)
    {
        var savedFilesData = new List<FileData>();

        foreach (var file in files)
        {
            if (file.Length <= 0)
            {
                continue;
            }

            var fileData = new FileData
            {
                Id = _guidGenerator.GenerateGuid(),
                Name = file.FileName
            };

            await using var stream = file.OpenReadStream();
            await _fileModelService.UploadFileAsync(fileData.Id.ToString(), stream, cancellationToken);

            savedFilesData.Add(fileData);
        }

        return savedFilesData;
    }

    private async Task DeletePreviousSavedFiles(ProjectModel projectModel, string loaderName,
        CancellationToken cancellationToken)
    {
        var filesToDelete = new List<string>();
        if (projectModel.SavedFiles.TryGetValue(loaderName, out var previousSavedFilesData))
        {
            filesToDelete.AddRange(previousSavedFilesData.Select(data => data.Id.ToString()));
        }

        await _fileModelService.DeleteFilesAsync(filesToDelete, cancellationToken);
    }
}
