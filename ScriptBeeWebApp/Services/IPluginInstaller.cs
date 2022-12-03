using System.Threading;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IPluginInstaller
{
    Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default);
    
    void UninstallPlugin(string pluginId, string pluginVersion);
}
