using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using ScriptBee.Adapters.Auth;
using ScriptBee.Adapters.Notifications.SignalR.Hubs;

namespace ScriptBee.Adapters.Notifications.SignalR.Tests.Hubs;

public class ProjectLiveUpdatesHubTests
{
    private readonly IGroupManager _groupManager = Substitute.For<IGroupManager>();
    private readonly HubCallerContext _context = Substitute.For<HubCallerContext>();
    private readonly ProjectLiveUpdatesHub _hub;

    public ProjectLiveUpdatesHubTests()
    {
        _context.ConnectionId.Returns("conn-123");
        _hub = new ProjectLiveUpdatesHub { Context = _context, Groups = _groupManager };
    }

    [Fact]
    public void JoinChannel_ShouldHaveAuthorizeActionAttributeWithProjectLiveUpdates()
    {
        // Arrange & Act
        var method = typeof(ProjectLiveUpdatesHub).GetMethod(
            nameof(ProjectLiveUpdatesHub.JoinChannel)
        );
        var attribute = method?.GetCustomAttribute<AuthorizeActionAttribute>();

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal("project:live_updates", attribute.Action);
    }

    [Fact]
    public void LeaveChannel_ShouldHaveAuthorizeActionAttributeWithProjectLiveUpdates()
    {
        // Arrange & Act
        var method = typeof(ProjectLiveUpdatesHub).GetMethod(
            nameof(ProjectLiveUpdatesHub.LeaveChannel)
        );
        var attribute = method?.GetCustomAttribute<AuthorizeActionAttribute>();

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal("project:live_updates", attribute.Action);
    }

    [Fact]
    public async Task JoinChannel_ShouldAddToCorrectGroup()
    {
        // Arrange
        const string projectId = "project-1";
        const string channelName = "scripts";
        const string expectedGroupName = "project-1_scripts";

        // Act
        await _hub.JoinChannel(projectId, channelName);

        // Assert
        await _groupManager
            .Received(1)
            .AddToGroupAsync("conn-123", expectedGroupName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LeaveChannel_ShouldRemoveFromCorrectGroup()
    {
        // Arrange
        const string projectId = "project-1";
        const string channelName = "analyses";
        const string expectedGroupName = "project-1_analyses";

        // Act
        await _hub.LeaveChannel(projectId, channelName);

        // Assert
        await _groupManager
            .Received(1)
            .RemoveFromGroupAsync("conn-123", expectedGroupName, Arg.Any<CancellationToken>());
    }
}
