namespace ScriptBee.ProjectContext;

public interface IFileWatcherService
{
    void SetupFileWatcher(string fullPath, string relativePath);
}
