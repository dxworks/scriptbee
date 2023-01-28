using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ScriptBeeWebApp.Data;

namespace ScriptBeeWebApp.Hubs;

public class FileWatcherHub : Hub<IFileWatcherClient>
{
    public override Task OnConnectedAsync()
    {
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return Task.CompletedTask;
    }

    public Task SendFileWatch(WatchedFile watchedFile)
    {
        return Clients.All.ReceiveFileWatch(watchedFile);
    }
}
