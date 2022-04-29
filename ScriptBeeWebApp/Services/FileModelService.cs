using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ScriptBeeWebApp.Services;

public class FileModelService : IFileModelService
{
    private IGridFSBucket bucket;

    public FileModelService(IMongoDatabase mongoDatabase)
    {
        this.bucket = new GridFSBucket(mongoDatabase);
    }

    public async Task UploadFile(string fileName, Stream fileStream)
    {
        await bucket.UploadFromStreamAsync(fileName, fileStream);
    }

    public async Task<Stream> GetFile(string fileName)
    {
        return await bucket.OpenDownloadStreamByNameAsync(fileName);
    }
}