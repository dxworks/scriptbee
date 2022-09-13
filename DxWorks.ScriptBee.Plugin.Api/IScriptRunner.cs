using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IScriptRunner : IPlugin
{
    public string Language { get; } // todo move in manifest.yaml

    public Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer, string scriptContent,
        CancellationToken cancellationToken = default);
}
