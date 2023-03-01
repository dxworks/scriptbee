using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class FileModelService : IFileModelService
{
    private readonly IGridFSBucket _bucket;

    public FileModelService(IMongoDatabase mongoDatabase)
    {
        _bucket = new GridFSBucket(mongoDatabase);
    }

    public async Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
    {
        await _bucket.UploadFromStreamAsync(fileName, fileStream, cancellationToken: cancellationToken);
    }

    public void UploadFile(string fileName, Stream fileStream)
    {
        _bucket.UploadFromStream(fileName, fileStream);
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        return await _bucket.OpenDownloadStreamByNameAsync(fileName);
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", fileName);
        var cursor = await _bucket.FindAsync(filter, cancellationToken: cancellationToken);
        var fileInfo = cursor.FirstOrDefault(cancellationToken);
        if (fileInfo != null)
        {
            await _bucket.DeleteAsync(fileInfo.Id, cancellationToken);
        }
    }

    public async Task DeleteFilesAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
    {
        var tasks = fileNames.Select(fileName => DeleteFileAsync(fileName, cancellationToken))
            .ToList();

        await Task.WhenAll(tasks);
    }
}
