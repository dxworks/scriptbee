using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Data;

namespace ScriptBeeWebApp.Services;

public class FileWatcherService : IFileWatcherService
{
    // todo needs an optimization to watch only the src folder of each project and notify accordingly all the files that are modified
    private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new();

    private readonly IFileWatcherHubService _fileWatcherHubService;

    public FileWatcherService(IFileWatcherHubService fileWatcherHubService)
    {
        _fileWatcherHubService = fileWatcherHubService;
    }

    public void SetupFileWatcher(string fullPath, string relativePath)
    {
        if (_watchers.ContainsKey(fullPath))
        {
            return;
        }

        var watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Path = Path.GetDirectoryName(fullPath)!,
            Filter = relativePath,
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
        };

        watcher.Changed += async (_, e) =>
        {
            try
            {
                await Task.Delay(100);

                var content = await File.ReadAllTextAsync(e.FullPath);

                var watchedFile = new WatchedFile(relativePath, content);
                await _fileWatcherHubService.SendFileWatch(watchedFile);
            }
            catch (Exception ex)
            {
                // todo log exception
                Console.WriteLine(ex);
            }
        };

        watcher.Error += (_, _) =>
        {
            // todo log and send to notification
            Console.WriteLine("error");
        };

        _watchers.AddOrUpdate(fullPath, watcher, (_, watcher1) => watcher1);
    }
}
