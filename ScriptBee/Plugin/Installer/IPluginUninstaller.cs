namespace ScriptBee.Plugin.Installer;

public interface IPluginUninstaller
{
    void ForceUninstall(string pathToPlugin);
    
    void Uninstall(string pathToPlugin);

    void DeleteMarkedPlugins();
}
