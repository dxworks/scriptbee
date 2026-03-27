namespace ScriptBee.Marketplace.Client;

public interface IPluginUrlFetcher
{
    string GetPluginUrl(string pluginId, string version);
}
