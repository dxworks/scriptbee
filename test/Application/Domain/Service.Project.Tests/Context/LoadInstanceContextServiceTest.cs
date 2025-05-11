using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class LoadInstanceContextServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly ILoadInstanceContext _loadInstanceContext =
        Substitute.For<ILoadInstanceContext>();

    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly LoadInstanceContextService _loadInstanceContextService;

    public LoadInstanceContextServiceTest()
    {
        _loadInstanceContextService = new LoadInstanceContextService(
            _getProject,
            _getProjectInstance,
            _loadInstanceContext,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenInstanceAndProject_ExpectContextToBeLoaded()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("aed2ef87-717c-4606-928a-314d39ad5e72");
        var command = new LoadContextCommand(projectId, instanceId, ["loader-id"]);
        var projectDetails = ProjectDetailsWithSavedFiles(
            projectId,
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("cfcc1094-9a12-49cb-ac98-0b7df523a1ab"), "file")]
                },
                {
                    "other",
                    [new FileData(new FileId("7dd98a7e-1c9b-425d-9fbb-c098bf3d786f"), "file")]
                },
            }
        );
        var instanceInfo = BasicInstanceInfo(projectId);
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

        var result = await _loadInstanceContextService.Load(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Unit());
        await _loadInstanceContext
            .Received(1)
            .Load(
                instanceInfo,
                Arg.Is<IDictionary<string, IEnumerable<FileId>>>(filesToLoad =>
                    filesToLoad.Count == 1
                    && filesToLoad["loader-id"]
                        .Single()
                        .Equals(new FileId("cfcc1094-9a12-49cb-ac98-0b7df523a1ab"))
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenInstanceAndProject_ExpectProjectLoadedFilesToBeUpdated()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("aed2ef87-717c-4606-928a-314d39ad5e72");
        var command = new LoadContextCommand(projectId, instanceId, ["loader-id"]);
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("cfcc1094-9a12-49cb-ac98-0b7df523a1ab"), "file")]
                },
                {
                    "other",
                    [new FileData(new FileId("7dd98a7e-1c9b-425d-9fbb-c098bf3d786f"), "file")]
                },
            },
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("e3138be2-1b24-4813-9785-e8dbce7043ad"), "file")]
                },
                {
                    "existing",
                    [new FileData(new FileId("6c4e9f85-499a-455e-b5d6-ce7134a57650"), "file")]
                },
            },
            []
        );
        var instanceInfo = BasicInstanceInfo(projectId);
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

        await _loadInstanceContextService.Load(command, TestContext.Current.CancellationToken);

        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    details.LoadedFiles.Count == 2
                    && details
                        .LoadedFiles["loader-id"]
                        .Single()
                        .Equals(
                            new FileData(new FileId("cfcc1094-9a12-49cb-ac98-0b7df523a1ab"), "file")
                        )
                    && details
                        .LoadedFiles["existing"]
                        .Single()
                        .Equals(
                            new FileData(new FileId("6c4e9f85-499a-455e-b5d6-ce7134a57650"), "file")
                        )
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("aed2ef87-717c-4606-928a-314d39ad5e72");
        var command = new LoadContextCommand(projectId, instanceId, ["loader-id"]);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        var result = await _loadInstanceContextService.Load(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("aed2ef87-717c-4606-928a-314d39ad5e72");
        var command = new LoadContextCommand(projectId, instanceId, ["loader-id"]);
        var projectDetails = BasicProjectDetails(projectId);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _loadInstanceContextService.Load(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT2.ShouldBe(instanceDoesNotExistsError);
    }
}
