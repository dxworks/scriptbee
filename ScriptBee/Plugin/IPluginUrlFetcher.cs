using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Plugin;

public interface IPluginUrlFetcher
{
    Task<string> GetPluginUrlAsync(string pluginId, string version, CancellationToken cancellationToken = default);
}
