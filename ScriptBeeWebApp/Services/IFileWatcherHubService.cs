using ScriptBeeWebApp.Data;

namespace ScriptBeeWebApp.Services;

public interface IFileWatcherHubService
{
    Task SendFileWatch(WatchedFile watchedFile);
}
