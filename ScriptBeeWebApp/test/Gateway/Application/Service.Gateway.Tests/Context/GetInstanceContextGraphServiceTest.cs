using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

namespace ScriptBee.Service.Gateway.Tests.Context;

public class GetInstanceContextGraphServiceTest
{
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();
    private readonly IGetInstanceContextGraph _getInstanceContextGraph =
        Substitute.For<IGetInstanceContextGraph>();
    private readonly GetInstanceContextGraphService _service;

    public GetInstanceContextGraphServiceTest()
    {
        _service = new GetInstanceContextGraphService(
            _getProjectInstance,
            _getInstanceContextGraph
        );
    }

    [Fact]
    public async Task SearchNodes_WhenInstanceExists_ReturnsResult()
    {
        // Arrange
        var instanceId = new InstanceId(Guid.NewGuid());
        var query = new GetInstanceContextGraphQuery(
            ProjectId.FromValue("project-id"),
            instanceId,
            "query",
            0,
            10
        );
        var instanceInfo = BasicInstanceInfo(query.ProjectId);
        var expectedResult = new ContextGraphResult([], []);

        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        _getInstanceContextGraph
            .SearchNodes(instanceInfo, "query", 0, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.SearchNodes(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task SearchNodes_WhenInstanceDoesNotExist_ReturnsError()
    {
        // Arrange
        var instanceId = new InstanceId(Guid.NewGuid());
        var query = new GetInstanceContextGraphQuery(
            ProjectId.FromValue("project-id"),
            instanceId,
            "query",
            0,
            10
        );
        var error = new InstanceDoesNotExistsError(instanceId);

        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(error));

        // Act
        var result = await _service.SearchNodes(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsT1.ShouldBeTrue();
        result.AsT1.ShouldBe(error);
    }

    [Fact]
    public async Task GetNeighbors_WhenInstanceExists_ReturnsResult()
    {
        // Arrange
        var instanceId = new InstanceId(Guid.NewGuid());
        var query = new GetInstanceContextNeighborsQuery(
            ProjectId.FromValue("project-id"),
            instanceId,
            "node-id"
        );
        var instanceInfo = BasicInstanceInfo(query.ProjectId);
        var expectedResult = new ContextGraphResult([], []);

        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        _getInstanceContextGraph
            .GetNeighbors(instanceInfo, "node-id", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.GetNeighbors(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task GetNeighbors_WhenInstanceDoesNotExist_ReturnsError()
    {
        // Arrange
        var instanceId = new InstanceId(Guid.NewGuid());
        var query = new GetInstanceContextNeighborsQuery(
            ProjectId.FromValue("project-id"),
            instanceId,
            "node-id"
        );
        var error = new InstanceDoesNotExistsError(instanceId);

        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(error));

        // Act
        var result = await _service.GetNeighbors(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsT1.ShouldBeTrue();
        result.AsT1.ShouldBe(error);
    }
}
