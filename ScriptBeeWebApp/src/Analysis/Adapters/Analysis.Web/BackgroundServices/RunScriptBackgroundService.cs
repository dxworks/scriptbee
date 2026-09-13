using System.Threading.Channels;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.BackgroundServices;

public class RunScriptBackgroundService(
    Channel<RunScriptRequest> runScriptChannel,
    IRunScriptService runScriptService,
    ILogger<RunScriptBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Run Script Background Service is starting");

        while (await runScriptChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            RunScriptRequest? request = null;
            try
            {
                request = await runScriptChannel.Reader.ReadAsync(stoppingToken);

                logger.LogInformation(
                    "Executing script {ScriptPath} for analysis {AnalysisId} on project {ProjectId}",
                    request.Script.File.Path,
                    request.AnalysisInfo.Id,
                    request.Script.ProjectId
                );

                await runScriptService.RunAsync(request, stoppingToken);

                logger.LogInformation(
                    "Script {ScriptPath} completed for analysis {AnalysisId}",
                    request.Script.File.Path,
                    request.AnalysisInfo.Id
                );
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Run Script Background Service is stopping");
                break;
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Unhandled error while executing script {ScriptPath} for analysis {AnalysisId}",
                    request?.Script.File.Path,
                    request?.AnalysisInfo.Id
                );
            }
        }

        logger.LogInformation("Run Script Background Service has stopped");
    }
}
