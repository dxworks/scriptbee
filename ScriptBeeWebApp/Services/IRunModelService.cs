using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptBeeWebApp.Models;

namespace ScriptBeeWebApp.Services;

public interface IRunModelService : IMongoService<RunModel>
{
    public Task<List<RunModel>> GetAllRunsForProject(ProjectModel projectModel, CancellationToken cancellationToken);
}