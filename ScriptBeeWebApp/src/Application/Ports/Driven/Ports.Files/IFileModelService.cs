using ScriptBee.Domain.Model.File;

namespace ScriptBee.Ports.Files;

public interface IFileModelService
{
    public Task UploadFileAsync(
        FileId fileId,
        Stream fileStream,
        CancellationToken cancellationToken = default
    );

    public void UploadFile(FileId fileId, Stream fileStream);

    public Task<Stream> GetFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task DeleteFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task DeleteFilesAsync(
        IEnumerable<FileId> fileIds,
        CancellationToken cancellationToken = default
    );
}
