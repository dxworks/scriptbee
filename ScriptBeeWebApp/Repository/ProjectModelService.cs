using MongoDB.Driver;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Repository;

public class ProjectModelService : MongoService<ProjectModel>, IProjectModelService
{
    private const string ProjectsCollectionName = "Projects";

    public ProjectModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<ProjectModel>(ProjectsCollectionName))
    {
    }
}
