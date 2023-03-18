namespace ScriptBee.Plugin;

public interface IPluginUrlFetcher
{
    string GetPluginUrl(string pluginId, string version);
}
