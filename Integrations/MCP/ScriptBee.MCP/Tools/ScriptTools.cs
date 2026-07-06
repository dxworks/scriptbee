using System.ComponentModel;
using ModelContextProtocol.Server;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Gateway.Generated.Contracts;

namespace ScriptBee.MCP.Tools;

[McpServerToolType]
public sealed class ScriptTools(IGatewayApi gatewayApi)
{
    [McpServerTool]
    [Description("Lists all scripts available in the specified project.")]
    public async Task<GetScriptDataResponse> GetScripts(
        [Description("The unique identifier of the project.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ScriptsGet(projectId, cancellationToken);

    [McpServerTool]
    [Description("Retrieves metadata about a specific script in a project.")]
    public async Task<ScriptData> GetScript(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the script.")] string scriptId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ScriptsGet2(projectId, scriptId, cancellationToken);

    [McpServerTool]
    [Description("Creates a new script within the specified project.")]
    public async Task<ScriptData> CreateScript(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The file path for the script within the project.")] string path,
        [Description("The scripting language (e.g. 'CSharp', 'Python', 'Javascript').")]
            string language,
        CancellationToken cancellationToken
    ) =>
        await gatewayApi.ScriptsPost(
            projectId,
            new CreateScriptCommand { Path = path, Language = language },
            cancellationToken
        );

    [McpServerTool]
    [Description("Updates the name or parameters of an existing script.")]
    public async Task<ScriptData> UpdateScript(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the script.")] string scriptId,
        [Description("The new name for the script.")] string name,
        CancellationToken cancellationToken
    ) =>
        await gatewayApi.ScriptsPatch(
            projectId,
            scriptId,
            new UpdateScriptCommand { Name = name },
            cancellationToken
        );

    [McpServerTool]
    [Description("Retrieves the source code content of a script.")]
    public async Task<string> GetScriptContent(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The unique identifier of the script.")] string scriptId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ContentGet(projectId, scriptId, cancellationToken);

    [McpServerTool]
    [Description("Lists all script types (languages) supported by the specified project.")]
    public async Task<GetAvailableScriptTypesResponse> GetAvailableScriptTypes(
        [Description("The unique identifier of the project.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.AvailableScriptTypes(projectId, cancellationToken);

    [McpServerTool]
    [Description("Lists all files and directories in the project file structure.")]
    public async Task<GetProjectFilesResponse> GetProjectFiles(
        [Description("The unique identifier of the project.")] string projectId,
        [Description("The parent node ID to list children of. Leave empty for the root.")]
            string? parentId,
        [Description("Number of items to skip (for pagination).")] int? offset,
        [Description("Maximum number of items to return.")] int? limit,
        CancellationToken cancellationToken
    ) => await gatewayApi.FilesGet(projectId, parentId ?? "", offset, limit, cancellationToken);
}
