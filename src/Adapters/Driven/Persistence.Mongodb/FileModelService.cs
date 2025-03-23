using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        return await _bucket.OpenDownloadStreamByNameAsync(
            fileName,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteFileAsync(
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", fileName);
        var cursor = await _bucket.FindAsync(filter, cancellationToken: cancellationToken);
        var fileInfo = cursor.FirstOrDefault(cancellationToken);
        if (fileInfo != null)
        {
            await _bucket.DeleteAsync(fileInfo.Id, cancellationToken);
        }
    }

    public async Task DeleteFilesAsync(
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = fileNames
            .Select(fileName => DeleteFileAsync(fileName, cancellationToken))
            .ToList();

        await Task.WhenAll(tasks);
    }
}
