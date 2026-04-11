using ScriptBee.Domain.Model.File;

namespace ScriptBee.Artifacts;

public interface IFileModelService
{
    public Task UploadFileAsync<TMetadata>(
        FileId fileId,
        Stream fileStream,
        TMetadata? metadata = null,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class;

    public void UploadFile<TMetadata>(FileId fileId, Stream fileStream, TMetadata? metadata = null)
        where TMetadata : class;

    public Task<Stream> GetFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task<TMetadata?> GetFileMetadataAsync<TMetadata>(
        FileId fileId,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class;

    public Task DeleteFileAsync(FileId fileId, CancellationToken cancellationToken = default);

    public Task DeleteFilesAsync(
        IEnumerable<FileId> fileIds,
        CancellationToken cancellationToken = default
    );
}
