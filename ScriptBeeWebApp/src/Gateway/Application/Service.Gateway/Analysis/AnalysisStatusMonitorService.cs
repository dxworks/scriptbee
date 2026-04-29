using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Notifications;
using ScriptBee.Ports.Notifications.Events;
using ScriptBee.Service.Gateway.Config;

namespace ScriptBee.Service.Gateway.Analysis;

public class AnalysisStatusMonitorService(
    IGetRunningAnalyses getRunningAnalyses,
    IGetAnalysis getAnalysis,
    IProjectNotificationsService projectNotificationsService,
    IOptions<ScriptBeeInstanceConfig> config,
    ILogger<AnalysisStatusMonitorService> logger
) : IAnalysisStatusMonitorService
{
    private readonly ConcurrentDictionary<
        AnalysisId,
        (ProjectId ProjectId, string LastStatus)
    > _trackedAnalyses = new();

    public void Track(AnalysisId analysisId, ProjectId projectId)
    {
        _trackedAnalyses.TryAdd(analysisId, (projectId, AnalysisStatus.Started.Value));
    }

    public async Task SeedRunningAnalyses(CancellationToken cancellationToken)
    {
        try
        {
            var runningAnalyses = await getRunningAnalyses.GetRunning(cancellationToken);
            foreach (var analysis in runningAnalyses)
            {
                _trackedAnalyses.TryAdd(analysis.Id, (analysis.ProjectId, analysis.Status.Value));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding running analyses");
        }
    }

    public async Task Monitor(CancellationToken cancellationToken)
    {
        var interval = TimeSpan.FromMilliseconds(
            config.Value.AnalysisStatusMonitorIntervalMilliseconds
        );

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, cancellationToken);

                await CheckAllTrackedAnalyses(cancellationToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while monitoring analysis status");
            }
        }
    }

    private async Task CheckAllTrackedAnalyses(CancellationToken cancellationToken)
    {
        foreach (var tracked in _trackedAnalyses)
        {
            await CheckAnalysisStatus(
                tracked.Key,
                tracked.Value.ProjectId,
                tracked.Value.LastStatus,
                cancellationToken
            );
        }
    }

    private async Task CheckAnalysisStatus(
        AnalysisId analysisId,
        ProjectId projectId,
        string lastStatus,
        CancellationToken cancellationToken
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        await result.Match(
            async analysisInfo =>
                await HandleStatusUpdate(analysisInfo, projectId, lastStatus, cancellationToken),
            error =>
            {
                _trackedAnalyses.TryRemove(analysisId, out _);
                return Task.CompletedTask;
            }
        );
    }

    private async Task HandleStatusUpdate(
        AnalysisInfo analysisInfo,
        ProjectId projectId,
        string lastStatus,
        CancellationToken cancellationToken
    )
    {
        if (analysisInfo.Status.Value == lastStatus)
        {
            return;
        }

        await projectNotificationsService.NotifyAnalysisStatusChanged(
            new AnalysisStatusChangedEvent(projectId, analysisInfo.Id, analysisInfo.Status.Value),
            cancellationToken
        );

        UpdateTrackedStatus(analysisInfo, projectId);
    }

    private void UpdateTrackedStatus(AnalysisInfo analysisInfo, ProjectId projectId)
    {
        if (
            analysisInfo.Status == AnalysisStatus.Finished
            || analysisInfo.Status == AnalysisStatus.Cancelled
        )
        {
            _trackedAnalyses.TryRemove(analysisInfo.Id, out _);
        }
        else
        {
            _trackedAnalyses[analysisInfo.Id] = (projectId, analysisInfo.Status.Value);
        }
    }
}
