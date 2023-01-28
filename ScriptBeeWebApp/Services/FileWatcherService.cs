using System.Collections.Concurrent;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Data;

namespace ScriptBeeWebApp.Services;

public class FileWatcherService : IFileWatcherService
{
    private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new();
    private readonly IFileWatcherHubService _fileWatcherHubService;
    private readonly Serilog.ILogger _logger;

    public FileWatcherService(IFileWatcherHubService fileWatcherHubService, Serilog.ILogger logger)
    {
        _fileWatcherHubService = fileWatcherHubService;
        _logger = logger;
    }

    public void RemoveFileWatcher(string fullPath)
    {
        _watchers.TryRemove(fullPath, out _);
    }

    public void SetupFileWatcher(string fullPath)
    {
        if (_watchers.ContainsKey(fullPath))
        {
            return;
        }

        var watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Path = Path.GetDirectoryName(fullPath)!,
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
        };

        watcher.Changed += async (_, e) =>
        {
            try
            {
                await using var fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs);

                var content = await sr.ReadToEndAsync();

                var watchedFile = new WatchedFile(e.FullPath, content);
                await _fileWatcherHubService.SendFileWatch(watchedFile);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while watching file");
            }
        };

        watcher.Error += (_, e) => _logger.Error(e.GetException(), "Error while watching file");

        _watchers.AddOrUpdate(fullPath, watcher, (_, watcher1) => watcher1);
    }
}
