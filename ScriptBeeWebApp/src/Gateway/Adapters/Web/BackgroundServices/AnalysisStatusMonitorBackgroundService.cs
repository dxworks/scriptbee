using ScriptBee.Service.Gateway.Analysis;

namespace ScriptBee.Web.BackgroundServices;

public class AnalysisStatusMonitorBackgroundService(
    IAnalysisStatusMonitorService analysisStatusMonitorService,
    ILogger<AnalysisStatusMonitorBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Analysis Status Monitor Background Service is starting");

        await analysisStatusMonitorService.SeedRunningAnalyses(stoppingToken);

        await analysisStatusMonitorService.Monitor(stoppingToken);

        logger.LogInformation("Analysis Status Monitor Background Service is stopping");
    }
}
