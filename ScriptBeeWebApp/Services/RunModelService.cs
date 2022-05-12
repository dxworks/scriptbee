using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public class RunModelService : MongoService<RunModel>, IRunModelService
{
    private const string RunsCollectionName = "Runs";

    public RunModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<RunModel>(RunsCollectionName))
    {
    }

    public async Task<List<RunModel>> GetAllRunsForProject(string projectId, CancellationToken cancellationToken)
    {
        var results = await mongoCollection.FindAsync(run => run.ProjectId == projectId,
            cancellationToken: cancellationToken);
        return results.ToList(cancellationToken);
    }
}