using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

// todo add tests with test containers
// todo update docs to reflect changes
public class RunModelService : MongoService<RunModel>, IRunModelService
{
    private const string RunsCollectionName = "Runs";

    public RunModelService(IMongoDatabase mongoDatabase) : base(
        mongoDatabase.GetCollection<RunModel>(RunsCollectionName))
    {
    }

    public async Task<IEnumerable<Run>> GetAllRunsForProject(string projectId,
        CancellationToken cancellationToken = default)
    {
        var asyncCursor =
            await mongoCollection.FindAsync(run => run.Id == projectId, cancellationToken: cancellationToken);

        var runModel = await asyncCursor.FirstOrDefaultAsync(cancellationToken);

        return runModel?.Runs ?? Enumerable.Empty<Run>();
    }

    public async Task AddRun(string projectId, Run run, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RunModel>.Filter.Eq(x => x.Id, projectId);

        var asyncCursor = await mongoCollection.FindAsync(filter, cancellationToken: cancellationToken);

        if (await asyncCursor.AnyAsync(cancellationToken: cancellationToken))
        {
            var update = Builders<RunModel>.Update.Push(x => x.Runs, run);

            await mongoCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
        else
        {
            await mongoCollection.InsertOneAsync(new RunModel
            {
                Id = projectId,
                Runs = new List<Run>
                {
                    run
                }
            }, cancellationToken: cancellationToken);
        }
    }
}
