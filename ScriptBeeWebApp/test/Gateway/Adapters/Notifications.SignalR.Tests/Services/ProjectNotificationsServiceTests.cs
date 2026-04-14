using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using ScriptBee.Adapters.Notifications.SignalR.Contracts;
using ScriptBee.Adapters.Notifications.SignalR.Hubs;
using ScriptBee.Adapters.Notifications.SignalR.Services;
using ScriptBee.Application.Model.Services;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Tests.Services;

public class ProjectNotificationsServiceTests
{
    private readonly IClientIdProvider _clientIdProvider = Substitute.For<IClientIdProvider>();
    private readonly IHubClients _hubClients = Substitute.For<IHubClients>();
    private readonly IClientProxy _clientProxy = Substitute.For<IClientProxy>();
    private readonly ProjectNotificationsService _service;

    public ProjectNotificationsServiceTests()
    {
        var hubContext = Substitute.For<IHubContext<ProjectLiveUpdatesHub>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = Substitute.For<HttpContext>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        hubContext.Clients.Returns(_hubClients);
        _hubClients.Group(Arg.Any<string>()).Returns(_clientProxy);
        _clientIdProvider.ClientId.Returns("test-client");

        httpContextAccessor.HttpContext.Returns(httpContext);
        httpContext.RequestServices.Returns(serviceProvider);
        serviceProvider.GetService(typeof(IClientIdProvider)).Returns(_clientIdProvider);

        _service = new ProjectNotificationsService(hubContext, httpContextAccessor);
    }

    [Fact]
    public async Task NotifyScriptCreated_SendsMessageToCorrectGroup()
    {
        // Arrange
        var projectId = ProjectId.FromValue("test-project");
        var scriptId = new ScriptId(Guid.NewGuid());
        var ev = new ScriptCreatedEvent(projectId, scriptId);

        // Act
        await _service.NotifyScriptCreated(ev, TestContext.Current.CancellationToken);

        // Assert
        _hubClients.Received(1).Group("test-project_scripts");
        await _clientProxy
            .Received(1)
            .SendCoreAsync(
                "ScriptCreated",
                Arg.Is<object[]>(args =>
                    ((SignalRScriptCreatedEvent)args[0]).ProjectId == ev.ProjectId.ToString()
                    && ((SignalRScriptCreatedEvent)args[0]).ScriptId == ev.ScriptId.ToString()
                    && ((SignalRScriptCreatedEvent)args[0]).ClientId == "test-client"
                ),
                TestContext.Current.CancellationToken
            );
    }

    [Fact]
    public async Task NotifyScriptUpdated_SendsMessageToCorrectGroup()
    {
        // Arrange
        var projectId = ProjectId.FromValue("test-project");
        var scriptId = new ScriptId(Guid.NewGuid());
        var ev = new ScriptUpdatedEvent(projectId, scriptId);

        // Act
        await _service.NotifyScriptUpdated(ev, TestContext.Current.CancellationToken);

        // Assert
        _hubClients.Received(1).Group("test-project_scripts");
        await _clientProxy
            .Received(1)
            .SendCoreAsync(
                "ScriptUpdated",
                Arg.Is<object[]>(args =>
                    ((SignalRScriptUpdatedEvent)args[0]).ProjectId == ev.ProjectId.ToString()
                    && ((SignalRScriptUpdatedEvent)args[0]).ScriptId == ev.ScriptId.ToString()
                    && ((SignalRScriptUpdatedEvent)args[0]).ClientId == "test-client"
                ),
                TestContext.Current.CancellationToken
            );
    }

    [Fact]
    public async Task NotifyScriptDeleted_SendsMessageToCorrectGroup()
    {
        // Arrange
        var projectId = ProjectId.FromValue("test-project");
        var scriptId = new ScriptId(Guid.NewGuid());
        var ev = new ScriptDeletedEvent(projectId, scriptId);

        // Act
        await _service.NotifyScriptDeleted(ev, TestContext.Current.CancellationToken);

        // Assert
        _hubClients.Received(1).Group("test-project_scripts");
        await _clientProxy
            .Received(1)
            .SendCoreAsync(
                "ScriptDeleted",
                Arg.Is<object[]>(args =>
                    ((SignalRScriptDeletedEvent)args[0]).ProjectId == ev.ProjectId.ToString()
                    && ((SignalRScriptDeletedEvent)args[0]).ScriptId == ev.ScriptId.ToString()
                    && ((SignalRScriptDeletedEvent)args[0]).ClientId == "test-client"
                ),
                TestContext.Current.CancellationToken
            );
    }
}
