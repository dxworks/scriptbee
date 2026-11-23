using Microsoft.Extensions.Logging;
using Refit;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class TriggerInstanceAnalysisAdapter(
    IHttpClientFactory httpClientFactory,
    ILogger<TriggerInstanceAnalysisAdapter> logger
) : ITriggerInstanceAnalysis
{
    public async Task<AnalysisInfo> Trigger(
        InstanceInfo instanceInfo,
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogDebug(
            "Triggering analysis for script {ScriptId} on instance {InstanceId} at {InstanceUrl}",
            scriptId,
            instanceInfo.Id,
            instanceInfo.Url
        );

        try
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(instanceInfo.Url);

            var analysisApi = RestService.For<IAnalysisApi>(client);

            var response = await analysisApi.Analyses(
                new RunAnalysisCommand(instanceInfo.ProjectId.ToString(), scriptId.ToString()),
                cancellationToken
            );

            logger.LogDebug(
                "Analysis {AnalysisId} triggered on instance {InstanceId}",
                response.Id,
                instanceInfo.Id
            );

            return new AnalysisInfo(
                new AnalysisId(response.Id),
                ProjectId.FromValue(response.ProjectId),
                instanceInfo.Id,
                new ScriptId(response.ScriptId),
                null,
                new AnalysisStatus(response.Status),
                [],
                [],
                response.CreationDate,
                null
            );
        }
        catch (Exception e) when (ExceptionUtils.IsConnectionRefused(e))
        {
            logger.LogError(
                e,
                "Connection refused when triggering analysis on instance {InstanceId} at {InstanceUrl}",
                instanceInfo.Id,
                instanceInfo.Url
            );
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to trigger analysis for script {ScriptId} on instance {InstanceId}",
                scriptId,
                instanceInfo.Id
            );
            throw;
        }
    }
}
