using System.Threading;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IProjectStructureService
{
    public Task<(string extension, string content)> GetSampleCodeAsync(string scriptType,
        CancellationToken cancellationToken = default);

    public Task GenerateModelClasses(string projectId, CancellationToken cancellationToken = default);
}
