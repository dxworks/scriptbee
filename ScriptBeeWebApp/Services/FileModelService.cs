using MongoDB.Driver;
using ScriptBeeWebApp.Models;

namespace ScriptBeeWebApp.Services;

public class FileModelService : MongoService<FileModel>, IFileModelService
{
    private const string FilesCollectionName = "Files";

    public FileModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<FileModel>(FilesCollectionName))
    {
    }
}