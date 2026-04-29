using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Analysis;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Notifications;
using ScriptBee.Ports.Notifications.Events;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.Service.Gateway.Config;
using static ScriptBee.Tests.Common.AnalysisInfoFixture;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class AnalysisStatusMonitorServiceTest
{
    private readonly IGetRunningAnalyses _getRunningAnalyses =
        Substitute.For<IGetRunningAnalyses>();
    private readonly IGetAnalysis _getAnalysis = Substitute.For<IGetAnalysis>();
    private readonly IProjectNotificationsService _projectNotificationsService =
        Substitute.For<IProjectNotificationsService>();
    private readonly IOptions<ScriptBeeInstanceConfig> _config = Substitute.For<
        IOptions<ScriptBeeInstanceConfig>
    >();
    private readonly ILogger<AnalysisStatusMonitorService> _logger = Substitute.For<
        ILogger<AnalysisStatusMonitorService>
    >();

    private readonly AnalysisStatusMonitorService _service;

    public AnalysisStatusMonitorServiceTest()
    {
        _config.Value.Returns(
            new ScriptBeeInstanceConfig { AnalysisStatusMonitorIntervalMilliseconds = 10 }
        );

        _service = new AnalysisStatusMonitorService(
            _getRunningAnalyses,
            _getAnalysis,
            _projectNotificationsService,
            _config,
            _logger
        );
    }

    [Fact]
    public async Task GivenRunningAnalysis_WhenStatusChanges_ThenNotifyAndStopTracking()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisInfoStarted = BasicAnalysisInfo(projectId);
        var analysisInfoFinished = analysisInfoStarted with { Status = AnalysisStatus.Finished };

        _getRunningAnalyses.GetRunning(Arg.Any<CancellationToken>()).Returns([analysisInfoStarted]);
        _getAnalysis
            .GetById(analysisInfoStarted.Id, Arg.Any<CancellationToken>())
            .Returns(analysisInfoFinished);

        await _service.SeedRunningAnalyses(TestContext.Current.CancellationToken);
        _ = _service.Monitor(TestContext.Current.CancellationToken);

        await Task.Delay(100, TestContext.Current.CancellationToken);

        await _projectNotificationsService
            .Received(1)
            .NotifyAnalysisStatusChanged(
                Arg.Is<AnalysisStatusChangedEvent>(e =>
                    e.AnalysisId == analysisInfoStarted.Id && e.Status == "Finished"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNewlyTrackedAnalysis_WhenStatusChanges_ThenNotify()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisInfoRunning = BasicAnalysisInfo(projectId, AnalysisStatus.Running);

        _getRunningAnalyses
            .GetRunning(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<AnalysisInfo>());
        _getAnalysis
            .GetById(analysisInfoRunning.Id, Arg.Any<CancellationToken>())
            .Returns(analysisInfoRunning);

        _service.Track(analysisInfoRunning.Id, projectId);

        _ = _service.Monitor(TestContext.Current.CancellationToken);

        await Task.Delay(100, TestContext.Current.CancellationToken);

        await _projectNotificationsService
            .Received(1)
            .NotifyAnalysisStatusChanged(
                Arg.Is<AnalysisStatusChangedEvent>(e =>
                    e.AnalysisId == analysisInfoRunning.Id && e.Status == "Running"
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
