using MongoDB.Driver;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Repository;

public class ScriptModelService : MongoService<ScriptModel>, IScriptModelService
{
    private const string ProjectsCollectionName = "Scripts";

    public ScriptModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<ScriptModel>(ProjectsCollectionName))
    {
    }

    public Task<ScriptModel?> GetScriptModelByFilePathAsync(string filePath, string projectId, CancellationToken cancellationToken = default)
    {
        // todo: add tests
        throw new NotImplementedException();
    }
}
