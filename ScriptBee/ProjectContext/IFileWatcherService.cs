namespace ScriptBee.ProjectContext;

public interface IFileWatcherService
{
    void SetupFileWatcher(string projectId, string fullPath);

    void RemoveFileWatcher(string projectId, string fullPath);
}
