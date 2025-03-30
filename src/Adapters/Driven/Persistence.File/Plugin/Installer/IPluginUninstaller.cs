namespace ScriptBee.Persistence.File.Plugin.Installer;

public interface IPluginUninstaller
{
    void ForceUninstall(string pathToPlugin);

    void Uninstall(string pathToPlugin);

    void DeleteMarkedPlugins();
}
