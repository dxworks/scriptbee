using ScriptBee.Models;

namespace ScriptBeeWebApp.Repository;

public interface IRunModelService : IMongoService<RunModel>
{
    Task<IEnumerable<Run>> GetAllRunsForProject(string projectId, CancellationToken cancellationToken = default);

    Task AddRun(string projectId, Run run, CancellationToken cancellationToken = default);
}
