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

    public async Task<ScriptModel?> GetScriptModelByFilePathAsync(string filePath, string projectId,
        CancellationToken cancellationToken = default)
    {
        var result = await mongoCollection.Find(x => x.FilePath == filePath && x.ProjectId == projectId)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }
}
