using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ScriptBee.Adapters.Auth;

namespace ScriptBee.Adapters.Notifications.SignalR.Hubs;

[Authorize]
public class ProjectLiveUpdatesHub : Hub
{
    [AuthorizeAction("project:live_updates")]
    public Task JoinChannel(string projectId, string channelName)
    {
        var groupName = $"{projectId}_{channelName}";
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    [AuthorizeAction("project:live_updates")]
    public Task LeaveChannel(string projectId, string channelName)
    {
        var groupName = $"{projectId}_{channelName}";
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
