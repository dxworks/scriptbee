using System.ComponentModel;
using ModelContextProtocol.Server;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Gateway.Generated.Contracts;

namespace ScriptBee.MCP.Tools;

[McpServerToolType]
public sealed class InstanceTools(IGatewayApi gatewayApi)
{
    [McpServerTool]
    [Description("Lists all execution instances associated with the specified project.")]
    public async Task<GetProjectInstancesListResponse> GetInstances(
        [Description("The unique identifier of the project.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.InstancesGet(projectId, cancellationToken);

    [McpServerTool]
    [Description("Retrieves detailed information about a specific project instance.")]
    public async Task<ProjectInstance> GetInstance(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.InstancesGet2(projectId, instanceId, cancellationToken);

    [McpServerTool]
    [Description("Allocates and adds a new execution instance for the specified project.")]
    public async Task<ProjectInstance> CreateInstance(
        [Description("The unique identifier of the project.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.InstancesPost(projectId, cancellationToken);

    [McpServerTool]
    [Description("Deallocates and removes a specific project instance.")]
    public async Task DeleteInstance(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the instance to remove.")] string instanceId,
        CancellationToken cancellationToken
    ) => await gatewayApi.InstancesDelete(projectId, instanceId, cancellationToken);
}
