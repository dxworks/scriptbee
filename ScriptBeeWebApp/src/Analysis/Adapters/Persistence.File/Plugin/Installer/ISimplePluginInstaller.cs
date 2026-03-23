namespace ScriptBee.Persistence.File.Plugin.Installer;

public interface ISimplePluginInstaller
{
    Task<string> Install(
        string url,
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}
