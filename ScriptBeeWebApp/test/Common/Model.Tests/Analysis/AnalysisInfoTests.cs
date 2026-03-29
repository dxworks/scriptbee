using System.Collections;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Tests.Analysis;

public class AnalysisInfoTests
{
    [Theory]
    [ClassData(typeof(StatusRunningTestData))]
    public void IsRunning(AnalysisStatus status, bool isRunning)
    {
        var analysisInfo = new AnalysisInfo(
            new AnalysisId(Guid.NewGuid()),
            ProjectId.FromValue("project-id"),
            new InstanceId(Guid.NewGuid()),
            new ScriptId(Guid.NewGuid()),
            new FileId(Guid.NewGuid()),
            status,
            [],
            [],
            DateTimeOffset.UtcNow,
            null
        );

        analysisInfo.IsRunning().ShouldBe(isRunning);
    }

    private class StatusRunningTestData : IEnumerable<TheoryDataRow<AnalysisStatus, bool>>
    {
        public IEnumerator<TheoryDataRow<AnalysisStatus, bool>> GetEnumerator()
        {
            yield return new TheoryDataRow<AnalysisStatus, bool>(AnalysisStatus.Started, true);
            yield return new TheoryDataRow<AnalysisStatus, bool>(AnalysisStatus.Running, true);
            yield return new TheoryDataRow<AnalysisStatus, bool>(AnalysisStatus.Finished, false);
            yield return new TheoryDataRow<AnalysisStatus, bool>(AnalysisStatus.Cancelled, false);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
