using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Plugin.Installer;

public interface IBundlePluginInstaller
{
    Task<List<string>> Install(string pluginId, string version, CancellationToken cancellationToken = default);
}
