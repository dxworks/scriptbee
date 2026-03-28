namespace ScriptBee.Rest.Contracts;

public class RestInstallPlugin
{
    public required string PluginId { get; set; }
    public required string Version { get; set; }
}
