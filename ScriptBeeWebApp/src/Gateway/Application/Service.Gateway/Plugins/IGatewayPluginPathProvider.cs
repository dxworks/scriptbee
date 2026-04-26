using ScriptBee.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

public interface IGatewayPluginPathProvider : IPluginPathProvider
{
    string GetInstallationFolderPath();
}
