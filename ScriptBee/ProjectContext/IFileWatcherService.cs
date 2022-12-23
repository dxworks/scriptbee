namespace ScriptBee.ProjectContext;

public interface IFileWatcherService
{
    void SetupFileWatcher(string fullPath);
    
    void RemoveFileWatcher(string fullPath);
}
