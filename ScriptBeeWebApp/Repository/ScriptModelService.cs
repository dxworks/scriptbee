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
}
