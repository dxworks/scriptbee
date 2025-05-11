using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class TriggerAnalysisServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly ITriggerInstanceAnalysis _triggerInstanceAnalysis =
        Substitute.For<ITriggerInstanceAnalysis>();

    private readonly TriggerAnalysisService _triggerAnalysisService;

    public TriggerAnalysisServiceTest()
    {
        _triggerAnalysisService = new TriggerAnalysisService(
            _getProjectInstance,
            _triggerInstanceAnalysis
        );
    }

    [Fact]
    public async Task GivenNoInstance_ThenReturnInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var command = new TriggerAnalysisCommand(
            projectId,
            instanceId,
            new ScriptId(Guid.NewGuid())
        );
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
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var analysisResult = await _triggerAnalysisService.Trigger(
            command,
            TestContext.Current.CancellationToken
        );

        analysisResult.AsT1.ShouldBe(new InstanceDoesNotExistsError(instanceId));
    }

    [Fact]
    public async Task GivenInstance_ThenTriggerSuccessful()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());
        var command = new TriggerAnalysisCommand(projectId, instanceId, scriptId);
        var analysisId = new AnalysisId(Guid.NewGuid());
        var instanceInfo = BasicInstanceInfo(projectId);
        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Started,
            [],
            [],
            DateTimeOffset.UtcNow,
            null
        );
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
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );
        _triggerInstanceAnalysis
            .Trigger(instanceInfo, scriptId, Arg.Any<CancellationToken>())
            .Returns(analysisInfo);

        var analysisResult = await _triggerAnalysisService.Trigger(
            command,
            TestContext.Current.CancellationToken
        );

        analysisResult.AsT0.ShouldBe(analysisInfo);
    }
}
