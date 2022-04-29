using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ScriptBeeWebApp.Services;

public class FileModelService : IFileModelService
{
    private readonly IGridFSBucket _bucket;

    public FileModelService(IMongoDatabase mongoDatabase)
    {
        this._bucket = new GridFSBucket(mongoDatabase);
    }

    public async Task UploadFile(string fileName, Stream fileStream, CancellationToken cancellationToken)
    {
        await _bucket.UploadFromStreamAsync(fileName, fileStream, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetFile(string fileName)
    {
        return await _bucket.OpenDownloadStreamByNameAsync(fileName);
    }
}