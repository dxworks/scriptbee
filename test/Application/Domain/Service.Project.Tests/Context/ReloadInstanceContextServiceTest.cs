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

public class ReloadInstanceContextServiceTest
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

    private readonly ReloadInstanceContextService _reloadInstanceContextService;

    public ReloadInstanceContextServiceTest()
    {
        _reloadInstanceContextService = new ReloadInstanceContextService(
            _getProject,
            _getProjectInstance,
            _clearInstanceContext,
            _loadInstanceContext,
            _linkInstanceContext
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectContextToBeReloaded()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new ReloadContextCommand(projectId, instanceId);
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader",
                    [new FileData(new FileId("38aaba34-6716-45ee-bb99-89450857516c"), "file.json")]
                },
            },
            ["linker"]
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

        var result = await _reloadInstanceContextService.Reload(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Unit());
        await _clearInstanceContext.Received(1).Clear(instanceInfo, Arg.Any<CancellationToken>());
        await _loadInstanceContext
            .Received(1)
            .Load(
                instanceInfo,
                Arg.Is<IDictionary<string, IEnumerable<FileId>>>(filesToLoad =>
                    filesToLoad.Count == 1
                    && filesToLoad["loader"]
                        .Single()
                        .Equals(new FileId("38aaba34-6716-45ee-bb99-89450857516c"))
                ),
                Arg.Any<CancellationToken>()
            );
        await _linkInstanceContext
            .Received(1)
            .Link(
                instanceInfo,
                Arg.Is<IEnumerable<string>>(x => x.Single().Equals("linker")),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new ReloadContextCommand(projectId, instanceId);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        var result = await _reloadInstanceContextService.Reload(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new ReloadContextCommand(projectId, instanceId);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _reloadInstanceContextService.Reload(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT2.ShouldBe(instanceDoesNotExistsError);
    }
}
