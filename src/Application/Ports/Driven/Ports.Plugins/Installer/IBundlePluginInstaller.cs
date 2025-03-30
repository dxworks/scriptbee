namespace ScriptBee.Ports.Plugins.Installer;

public interface IBundlePluginInstaller
{
    Task<List<string>> Install(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}
