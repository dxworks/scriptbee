using Microsoft.AspNetCore.SignalR;

namespace ScriptBee.Adapters.Notifications.SignalR.Hubs;

public class ProjectLiveUpdatesHub : Hub
{
    public Task JoinChannel(string projectId, string channelName)
    {
        var groupName = $"{projectId}_{channelName}";
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task LeaveChannel(string projectId, string channelName)
    {
        var groupName = $"{projectId}_{channelName}";
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
