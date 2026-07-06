using System.ComponentModel;
using ModelContextProtocol.Server;
using Refit;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Gateway.Generated.Contracts;

namespace ScriptBee.MCP.Tools;

[McpServerToolType]
public sealed class ProjectTools(IGatewayApi gatewayApi)
{
    [McpServerTool]
    [Description("Lists all existing ScriptBee projects with their basic details.")]
    public async Task<GetProjectListResponse> GetProjects(CancellationToken cancellationToken) =>
        await gatewayApi.ProjectsGet(cancellationToken);

    [McpServerTool]
    [Description("Retrieves detailed information about a specific ScriptBee project.")]
    public async Task<ProjectDetails> GetProject(
        [Description("The unique identifier of the project.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ProjectsGet2(projectId, cancellationToken);

    [McpServerTool]
    [Description("Creates a new ScriptBee project with the specified ID and name.")]
    public async Task<CreateProjectResponse> CreateProject(
        [Description("A unique identifier for the new project.")] string id,
        [Description("A human-readable name for the new project.")] string name,
        CancellationToken cancellationToken
    ) =>
        await gatewayApi.ProjectsPost(
            new CreateProjectCommand { Id = id, Name = name },
            cancellationToken
        );

    [McpServerTool]
    [Description("Deletes a ScriptBee project and all its associated data.")]
    public async Task DeleteProject(
        [Description("The unique identifier of the project to delete.")] string projectId,
        CancellationToken cancellationToken
    ) => await gatewayApi.ProjectsDelete(projectId, cancellationToken);
}
