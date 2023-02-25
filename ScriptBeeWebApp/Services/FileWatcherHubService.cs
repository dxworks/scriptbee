using Microsoft.AspNetCore.SignalR;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.Hubs;

namespace ScriptBeeWebApp.Services;

public class FileWatcherHubService : IFileWatcherHubService
{
    private readonly IHubContext<FileWatcherHub, IFileWatcherClient> _hubContext;

    public FileWatcherHubService(IHubContext<FileWatcherHub, IFileWatcherClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendFileWatch(WatchedFile watchedFile)
    {
        await _hubContext.Clients.All.ReceiveFileWatch(watchedFile);
    }
}
