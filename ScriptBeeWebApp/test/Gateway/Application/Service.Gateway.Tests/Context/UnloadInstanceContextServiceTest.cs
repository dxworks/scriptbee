using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Gateway.Tests.Context;

public class UnloadInstanceContextServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();
    private readonly IClearInstanceContext _clearInstanceContext =
        Substitute.For<IClearInstanceContext>();
    private readonly ILoadInstanceContext _loadInstanceContext =
        Substitute.For<ILoadInstanceContext>();
    private readonly ILinkInstanceContext _linkInstanceContext =
        Substitute.For<ILinkInstanceContext>();
    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly UnloadInstanceContextService _service;

    public UnloadInstanceContextServiceTest()
    {
        _service = new UnloadInstanceContextService(
            _getProject,
            _getProjectInstance,
            _clearInstanceContext,
            _loadInstanceContext,
            _linkInstanceContext,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenNoProject_ExpectProjectDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var command = new UnloadInstanceContextCommand(projectId, instanceId, "loader-id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        // Act
        var result = await _service.Unload(command, TestContext.Current.CancellationToken);

        // Assert
        result.AsT1.ShouldBe(new ProjectDoesNotExistsError(projectId));
    }

    [Fact]
    public async Task GivenNoInstance_ExpectInstanceDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var command = new UnloadInstanceContextCommand(projectId, instanceId, "loader-id");
        var projectDetails = BasicProjectDetails(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        // Act
        var result = await _service.Unload(command, TestContext.Current.CancellationToken);

        // Assert
        result.AsT2.ShouldBe(new InstanceDoesNotExistsError(instanceId));
    }

    [Fact]
    public async Task GivenFileSpecified_ExpectFileRemovedAndContextReloaded()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var fileIdToRemove = new FileId("cf34dc92-b1cf-4e0b-86a0-3227c22f5196");
        var otherFileId = new FileId("17d16d43-bb4d-428a-80da-bd752544a9ac");
        var command = new UnloadInstanceContextCommand(
            projectId,
            instanceId,
            "loader-1",
            fileIdToRemove
        );
        var instanceInfo = BasicInstanceInfo(projectId);
        var projectDetails = new ProjectDetails(
            projectId,
            "project",
            DateTimeOffset.UtcNow,
            [],
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-1",
                    [new FileData(fileIdToRemove, "name-1"), new FileData(otherFileId, "name-2")]
                },
                { "loader-2", [new FileData(new FileId(Guid.NewGuid()), "name-3")] },
            },
            ["linker-1"],
            []
        );
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        // Act
        var result = await _service.Unload(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsT0.ShouldBeTrue();
        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    details.LoadedFiles["loader-1"].Count == 1
                    && details.LoadedFiles["loader-1"][0].Id.Equals(otherFileId)
                    && details.LoadedFiles["loader-2"].Count == 1
                ),
                Arg.Any<CancellationToken>()
            );
        await _clearInstanceContext.Received(1).Clear(instanceInfo, Arg.Any<CancellationToken>());
        await _loadInstanceContext
            .Received(1)
            .Load(
                instanceInfo,
                Arg.Is<IDictionary<string, IEnumerable<FileId>>>(dict =>
                    dict.Count == 2 && dict["loader-1"].Single().Equals(otherFileId)
                ),
                Arg.Any<CancellationToken>()
            );
        await _linkInstanceContext
            .Received(1)
            .Link(
                instanceInfo,
                Arg.Is<IEnumerable<string>>(l => l.Contains("linker-1")),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNoFileSpecified_ExpectEntireLoaderRemovedAndContextReloaded()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var command = new UnloadInstanceContextCommand(projectId, instanceId, "loader-1");
        var instanceInfo = BasicInstanceInfo(projectId);
        var projectDetails = new ProjectDetails(
            projectId,
            "project",
            DateTimeOffset.UtcNow,
            [],
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-1",
                    [new FileData(new FileId("cf34dc92-b1cf-4e0b-86a0-3227c22f5196"), "name-1")]
                },
                {
                    "loader-2",
                    [new FileData(new FileId("17d16d43-bb4d-428a-80da-bd752544a9ac"), "name-2")]
                },
            },
            [],
            []
        );
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        // Act
        var result = await _service.Unload(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsT0.ShouldBeTrue();
        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    !details.LoadedFiles.ContainsKey("loader-1")
                    && details.LoadedFiles.ContainsKey("loader-2")
                ),
                Arg.Any<CancellationToken>()
            );
        await _clearInstanceContext.Received(1).Clear(instanceInfo, Arg.Any<CancellationToken>());
        await _loadInstanceContext
            .Received(1)
            .Load(
                instanceInfo,
                Arg.Is<IDictionary<string, IEnumerable<FileId>>>(dict =>
                    dict.Count == 1 && dict.ContainsKey("loader-2")
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
