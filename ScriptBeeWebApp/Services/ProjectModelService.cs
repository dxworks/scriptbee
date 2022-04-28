using MongoDB.Driver;
using ScriptBeeWebApp.Models;

namespace ScriptBeeWebApp.Services;

public class ProjectModelService : MongoService<ProjectModel>, IProjectModelService
{
    private const string ProjectsCollectionName = "Projects";

    public ProjectModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<ProjectModel>(ProjectsCollectionName))
    {
    }
}