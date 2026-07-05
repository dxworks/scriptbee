using Refit;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class TriggerInstanceAnalysisAdapter(IHttpClientFactory httpClientFactory)
    : ITriggerInstanceAnalysis
{
    public async Task<AnalysisInfo> Trigger(
        InstanceInfo instanceInfo,
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var analysisApi = RestService.For<IAnalysisApi>(client);

        var response = await analysisApi.Analyses(
            new RunAnalysisCommand(instanceInfo.ProjectId.ToString(), scriptId.ToString()),
            cancellationToken
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
}
