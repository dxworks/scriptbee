using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class TriggerAnalysisServiceTest
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();

    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IAllocateInstance _allocateInstance = Substitute.For<IAllocateInstance>();

    private readonly TriggerAnalysisService _triggerAnalysisService;

    public TriggerAnalysisServiceTest()
    {
        _triggerAnalysisService = new TriggerAnalysisService(
            _dateTimeProvider,
            _guidProvider,
            _getAllProjectInstances,
            _allocateInstance
        );
    }

    [Fact]
    public async Task GivenNoAllocatedInstance_ThenTriggerSuccessful()
    {
        var creationDate = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var command = new TriggerAnalysisCommand(
            projectId,
            new AnalysisInstanceImage("image"),
            ["loader"],
            ["linker"]
        );
        var instanceIdGuid = Guid.NewGuid();
        var analysisIdGuid = Guid.NewGuid();
        var analysisId = new AnalysisId(analysisIdGuid);
        _dateTimeProvider.UtcNow().Returns(creationDate);
        _guidProvider.NewGuid().Returns(instanceIdGuid, analysisIdGuid);
        _getAllProjectInstances
            .GetAll(projectId)
            .Returns(Task.FromResult<IEnumerable<InstanceInfo>>(new List<InstanceInfo>()));
        _allocateInstance.Allocate(new AnalysisInstanceImage("image")).Returns("http://instance");

        var analysisResult = await _triggerAnalysisService.Trigger(command);

        analysisResult.Id.ShouldBeEquivalentTo(analysisId);
        analysisResult.ProjectId.ShouldBe(projectId);
        analysisResult.Status.ShouldBe(AnalysisStatus.Started);
        analysisResult.Results.ShouldBeEmpty();
        analysisResult.Errors.ShouldBeEmpty();
        analysisResult.CreationDate.ShouldBe(creationDate);
        analysisResult.FinishedDate.ShouldBeNull();
    }

    [Fact]
    public async Task GivenAllocatedInstance_ThenTriggerSuccessful()
    {
        var creationDate = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var command = new TriggerAnalysisCommand(
            projectId,
            new AnalysisInstanceImage("image"),
            ["loader"],
            ["linker"]
        );
        var analysisIdGuid = Guid.NewGuid();
        var analysisId = new AnalysisId(analysisIdGuid);
        var instanceInfo = new InstanceInfo(
            new InstanceId(Guid.NewGuid()),
            projectId,
            "http://instance",
            DateTimeOffset.Now
        );
        _dateTimeProvider.UtcNow().Returns(creationDate);
        _guidProvider.NewGuid().Returns(analysisIdGuid);
        _getAllProjectInstances
            .GetAll(projectId)
            .Returns(
                Task.FromResult<IEnumerable<InstanceInfo>>(new List<InstanceInfo> { instanceInfo })
            );

        var analysisResult = await _triggerAnalysisService.Trigger(command);

        analysisResult.Id.ShouldBeEquivalentTo(analysisId);
        analysisResult.ProjectId.ShouldBe(projectId);
        analysisResult.Status.ShouldBe(AnalysisStatus.Started);
        analysisResult.Results.ShouldBeEmpty();
        analysisResult.Errors.ShouldBeEmpty();
        analysisResult.CreationDate.ShouldBe(creationDate);
        analysisResult.FinishedDate.ShouldBeNull();

        await _allocateInstance
            .Received(0)
            .Allocate(Arg.Any<AnalysisInstanceImage>(), Arg.Any<CancellationToken>());
    }
}
