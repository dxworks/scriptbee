using DxWorks.ScriptBee.Analysis.Sdk.State;
using NSubstitute;
using ScriptBee.Analysis;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.State;

public class AnalysisStateTests
{
    private readonly ICreateAnalysis _createAnalysis = Substitute.For<ICreateAnalysis>();
    private readonly IUpdateAnalysis _updateAnalysis = Substitute.For<IUpdateAnalysis>();

    private readonly AnalysisState _state;

    public AnalysisStateTests()
    {
        _state = new AnalysisState(_createAnalysis, _updateAnalysis);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_UsesCreateAnalysis()
    {
        var analysisInfo = CreateAnalysisInfo();
        _createAnalysis.Create(analysisInfo, Arg.Any<CancellationToken>()).Returns(analysisInfo);

        var result = await _state.CreateAsync(analysisInfo, TestContext.Current.CancellationToken);

        result.ShouldBeSameAs(analysisInfo);
        await _createAnalysis.Received(1).Create(analysisInfo, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_UsesUpdateAnalysis()
    {
        var analysisInfo = CreateAnalysisInfo();

        await _state.UpdateAsync(analysisInfo, TestContext.Current.CancellationToken);

        await _updateAnalysis.Received(1).Update(analysisInfo, Arg.Any<CancellationToken>());
    }

    private static AnalysisInfo CreateAnalysisInfo()
    {
        return new AnalysisInfo(
            new AnalysisId("41f5bfb5-e1cd-4f2b-a56f-dc877a29365a"),
            ProjectId.FromValue("project-id"),
            new InstanceId("0ef037fc-7cff-4aaf-a3db-73bc9d17a35e"),
            new ScriptId("7e715fe3-6f10-48d0-92a1-3d960d32d7c4"),
            null,
            AnalysisStatus.Started,
            [],
            [],
            DateTimeOffset.UtcNow,
            null
        );
    }
}
