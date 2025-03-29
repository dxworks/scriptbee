using Refit;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Api;

public interface IAnalysisApi
{
    [Post("/api/analyses")]
    Task<RestRunAnalysisResponse> TriggerAnalysis(
        [Body] RestRunAnalysisCommand request,
        CancellationToken cancellationToken
    );
}
