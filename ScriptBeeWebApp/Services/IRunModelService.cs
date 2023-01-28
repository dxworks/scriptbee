using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IRunModelService : IMongoService<RunModel>
{
    Task<IEnumerable<Run>> GetAllRunsForProject(string projectId, CancellationToken cancellationToken = default);

    Task AddRun(string projectId, Run run, CancellationToken cancellationToken = default);
}
