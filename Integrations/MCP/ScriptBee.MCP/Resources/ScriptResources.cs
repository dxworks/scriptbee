using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ScriptBee.MCP.Gateway.Generated;

namespace ScriptBee.MCP.Resources;

[McpServerResourceType]
public sealed class ScriptResources(IGatewayApi gatewayApi)
{
    [McpServerResource(
        UriTemplate = "scriptbee://projects/{projectId}/scripts/{scriptId}/content",
        Name = "Script Source Code",
        MimeType = "text/plain"
    )]
    public async Task<string> GetScriptContent(
        string projectId,
        string scriptId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ContentGet(projectId, scriptId, cancellationToken);

    [McpServerResource(
        UriTemplate = "scriptbee://projects/{projectId}/analyses/{analysisId}/console",
        Name = "Analysis Console Output",
        MimeType = "text/plain"
    )]
    public async Task<string> GetAnalysisConsole(
        string projectId,
        string analysisId,
        CancellationToken cancellationToken
    )
    {
        var result = await gatewayApi.Console(projectId, analysisId, cancellationToken);
        return result.Content;
    }

    [McpServerResource(
        UriTemplate = "scriptbee://projects/{projectId}/instances/{instanceId}/context",
        Name = "Instance Context",
        MimeType = "application/json"
    )]
    public async Task<string> GetInstanceContext(
        string projectId,
        string instanceId,
        CancellationToken cancellationToken
    )
    {
        var context = await gatewayApi.Context(projectId, instanceId, cancellationToken);
        return JsonSerializer.Serialize(
            context,
            new JsonSerializerOptions { WriteIndented = true }
        );
    }
}
