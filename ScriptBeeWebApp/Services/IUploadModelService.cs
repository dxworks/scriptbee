using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IUploadModelService
{
    Task<List<FileData>> UploadFilesAsync(ProjectModel projectModel, string loaderName, IFormFileCollection files,
        CancellationToken cancellationToken = default);
}
