using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace DxWorks.ScriptBee.Plugin.Api.ScriptRunner;

public interface IScriptRunner
{
    public Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default);
}
