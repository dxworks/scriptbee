using Refit;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Api;

public interface IContextApi
{
    [Get("/api/context")]
    Task<RestGetContextResponse> Get(CancellationToken cancellationToken);

    [Post("/api/context/clear")]
    Task Clear(CancellationToken cancellationToken);

    [Post("/api/context/link")]
    Task Link([Body] RestContextLink request, CancellationToken cancellationToken);

    [Post("/api/context/load")]
    Task Load([Body] RestContextLoad request, CancellationToken cancellationToken);

    [Post("/api/context/generate-classes")]
    Task<Stream> GenerateClasses(
        [Body] RestGenerateClasses request,
        CancellationToken cancellationToken
    );

    [Get("/api/context/graph-nodes")]
    Task<RestContextGraphResponse> SearchNodes(
        [Query] string? query,
        [Query] int offset,
        [Query] int limit,
        CancellationToken cancellationToken
    );

    [Get("/api/context/graph-nodes/{nodeId}/neighbors")]
    Task<RestContextGraphResponse> GetNeighbors(
        [AliasAs("nodeId")] string nodeId,
        CancellationToken cancellationToken
    );
}
