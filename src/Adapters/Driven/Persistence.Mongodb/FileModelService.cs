using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ScriptBee.Domain.Model.File;
using ScriptBee.Ports.Files;

namespace ScriptBee.Persistence.Mongodb;

public class FileModelService(IMongoDatabase mongoDatabase) : IFileModelService
{
    private readonly GridFSBucket _bucket = new(mongoDatabase);

    public async Task UploadFileAsync(
        string fileName,
        Stream fileStream,
        CancellationToken cancellationToken = default
    )
    {
        await _bucket.UploadFromStreamAsync(
            fileName,
            fileStream,
            cancellationToken: cancellationToken
        );
    }

    public void UploadFile(string fileName, Stream fileStream)
    {
        _bucket.UploadFromStream(fileName, fileStream);
    }

    public async Task<Stream> GetFileAsync(
        FileId fileId,
        CancellationToken cancellationToken = default
    )
    {
        return await _bucket.OpenDownloadStreamByNameAsync(
            fileId.ToString(),
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteFileAsync(FileId fileId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", fileId.ToString());
        var cursor = await _bucket.FindAsync(filter, cancellationToken: cancellationToken);
        var fileInfo = cursor.FirstOrDefault(cancellationToken);
        if (fileInfo != null)
        {
            await _bucket.DeleteAsync(fileInfo.Id, cancellationToken);
        }
    }

    public async Task DeleteFilesAsync(
        IEnumerable<FileId> fileIds,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = fileIds.Select(fileId => DeleteFileAsync(fileId, cancellationToken)).ToList();

        await Task.WhenAll(tasks);
    }
}
