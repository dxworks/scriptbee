namespace ScriptBee.UseCases.Plugin;

public interface IInstallPluginUseCase
{
    Task InstallPlugin(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}
