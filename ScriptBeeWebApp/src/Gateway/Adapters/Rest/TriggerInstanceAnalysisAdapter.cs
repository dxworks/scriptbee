using Refit;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;
using ScriptBee.Rest.Contracts;

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

        var contextApi = RestService.For<IAnalysisApi>(client);

        var response = await contextApi.TriggerAnalysis(
            new RestRunAnalysisCommand
            {
                ProjectId = instanceInfo.ProjectId.ToString(),
                ScriptId = scriptId.ToString(),
            },
            cancellationToken
        );

        return response.MapToAnalysisInfo();
    }
}
