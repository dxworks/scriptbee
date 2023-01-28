using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Plugin.Installer;

public interface ISimplePluginInstaller
{
    Task<string> Install(string url, string pluginId, string version, CancellationToken cancellationToken = default);
}
