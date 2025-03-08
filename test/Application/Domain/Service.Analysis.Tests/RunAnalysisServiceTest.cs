using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class RunAnalysisServiceTest
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly ICreateAnalysis _createAnalysis = Substitute.For<ICreateAnalysis>();

    private readonly RunAnalysisService _runAnalysisService;

    public RunAnalysisServiceTest()
    {
        _runAnalysisService = new RunAnalysisService(
            _dateTimeProvider,
            _guidProvider,
            _createAnalysis
        );
    }

    [Fact]
    public async Task CreateAnalysisSuccessful()
    {
        var creationDate = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());
        var command = new RunAnalysisCommand(projectId, scriptId);
        _dateTimeProvider.UtcNow().Returns(creationDate);
        _guidProvider.NewGuid().Returns(analysisId.Value);

        var analysisResult = await _runAnalysisService.Run(command);

        analysisResult.Id.ShouldBeEquivalentTo(analysisId);
        analysisResult.ProjectId.ShouldBe(projectId);
        analysisResult.ScriptId.ShouldBe(scriptId);
        analysisResult.Status.ShouldBe(AnalysisStatus.Started);
        analysisResult.Results.ShouldBeEmpty();
        analysisResult.Errors.ShouldBeEmpty();
        analysisResult.CreationDate.ShouldBe(creationDate);
        analysisResult.FinishedDate.ShouldBeNull();
    }
}
