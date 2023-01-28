using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IHelperFunctions : IPlugin
{
    Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    Task OnUnloadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
