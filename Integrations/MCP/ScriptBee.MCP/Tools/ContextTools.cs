using System.ComponentModel;
using ModelContextProtocol.Server;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Gateway.Generated.Contracts;

namespace ScriptBee.MCP.Tools;

[McpServerToolType]
public sealed class ContextTools(IGatewayApi gatewayApi)
{
    [McpServerTool]
    [Description("Lists all data loaders available for the specified project instance.")]
    public async Task<GetLoadersResponse> GetLoaders(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Loaders(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description(
        "Loads data into the context of the specified instance using the selected loaders."
    )]
    public async Task LoadContext(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        [Description("List of loader IDs to use for loading.")] IEnumerable<string> loaderIds,
        CancellationToken cancellationToken
    ) =>
        await gatewayApi.Load(
            projectId,
            instanceId,
            new LoadContextCommand { LoaderIds = loaderIds.ToList() },
            cancellationToken
        );

    [McpServerTool]
    [Description("Lists all linkers available for the specified project instance.")]
    public async Task<GetLinkersResponse> GetLinkers(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Linkers(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description("Links the loaded data context using the selected linkers.")]
    public async Task LinkContext(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        [Description("List of linker IDs to apply.")] IEnumerable<string> linkerIds,
        CancellationToken cancellationToken
    ) =>
        await gatewayApi.Link(
            projectId,
            instanceId,
            new LinkContextCommand { LinkerIds = linkerIds.ToList() },
            cancellationToken
        );

    [McpServerTool]
    [Description(
        "Reloads the data context for the specified instance, refreshing all loaded and linked data."
    )]
    public async Task ReloadContext(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Reload(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description("Retrieves the current data context for the specified instance.")]
    public async Task<GetProjectContextResponse> GetContext(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Context(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description("Clears all loaded data from the context of the specified instance.")]
    public async Task ClearContext(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Clear(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description("Searches the context graph for nodes matching the given query string.")]
    public async Task<ContextGraphResponse> SearchContextGraph(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        [Description("The search query to filter nodes.")] string query,
        [Description("Number of results to skip.")] int? offset,
        [Description("Maximum number of results to return.")] int? limit,
        CancellationToken cancellationToken
    ) => await gatewayApi.Graph(projectId, instanceId, query, offset, limit, cancellationToken);

    [McpServerTool]
    [Description("Retrieves the immediate neighbours and edges for a specific context graph node.")]
    public async Task<ContextGraphResponse> GetContextNodeNeighbours(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        [Description("The ID of the node whose neighbours to retrieve.")] string nodeId,
        CancellationToken cancellationToken
    ) => await gatewayApi.Neighbors(projectId, instanceId, nodeId, cancellationToken);
}
