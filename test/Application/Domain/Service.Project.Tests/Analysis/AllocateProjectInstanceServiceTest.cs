using NSubstitute;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.Tests.Common;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class AllocateProjectInstanceServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IAllocateInstance _allocateInstance = Substitute.For<IAllocateInstance>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private readonly ICreateProjectInstance _createProjectInstance =
        Substitute.For<ICreateProjectInstance>();

    private readonly IInstanceTemplateProvider _instanceTemplateProvider =
        Substitute.For<IInstanceTemplateProvider>();

    private readonly AllocateProjectInstanceService _allocateProjectInstanceService;

    public AllocateProjectInstanceServiceTest()
    {
        _allocateProjectInstanceService = new AllocateProjectInstanceService(
            _getProject,
            _allocateInstance,
            _guidProvider,
            _dateTimeProvider,
            _createProjectInstance,
            _instanceTemplateProvider
        );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistError()
    {
        var projectId = ProjectId.FromValue("project-id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var result = await _allocateProjectInstanceService.Allocate(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new ProjectDoesNotExistsError(projectId));
    }

    [Fact]
    public async Task GivenProject_ExpectInstanceToBeCreatedAndAllocated()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = Guid.NewGuid();
        var createdDate = DateTimeOffset.UtcNow;
        var instanceInfo = new InstanceInfo(
            new InstanceId(instanceId),
            projectId,
            "http://instance-url",
            createdDate
        );
        var analysisInstanceImage = new AnalysisInstanceImage("scriptbee/analysis:latest");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    ProjectDetailsFixture.BasicProjectDetails(projectId)
                )
            );
        _allocateInstance
            .Allocate(
                new InstanceId(instanceId),
                analysisInstanceImage,
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult("http://instance-url"));
        _guidProvider.NewGuid().Returns(instanceId);
        _dateTimeProvider.UtcNow().Returns(createdDate);
        _instanceTemplateProvider.GetTemplate().Returns(analysisInstanceImage);
        _createProjectInstance
            .Create(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(instanceInfo));

        var result = await _allocateProjectInstanceService.Allocate(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(instanceInfo);
    }
}
