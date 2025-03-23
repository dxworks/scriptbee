using ScriptBee.Domain.Model.File;

namespace ScriptBee.Ports.Files;

public interface IFileModelService
{
    public Task UploadFileAsync(
        string fileName,
        Stream fileStream,
        CancellationToken cancellationToken = default
    );

    public void UploadFile(string fileName, Stream fileStream);

    public Task<Stream> GetFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task DeleteFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task DeleteFilesAsync(
        IEnumerable<FileId> fileIds,
        CancellationToken cancellationToken = default
    );
}
