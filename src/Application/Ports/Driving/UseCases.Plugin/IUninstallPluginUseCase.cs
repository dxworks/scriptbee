namespace ScriptBee.UseCases.Plugin;

public interface IUninstallPluginUseCase
{
    void UninstallPlugin(string pluginId, string pluginVersion);
}
