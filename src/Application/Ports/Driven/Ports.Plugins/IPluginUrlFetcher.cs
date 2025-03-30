namespace ScriptBee.Ports.Plugins;

public interface IPluginUrlFetcher
{
    string GetPluginUrl(string pluginId, string version);
}
