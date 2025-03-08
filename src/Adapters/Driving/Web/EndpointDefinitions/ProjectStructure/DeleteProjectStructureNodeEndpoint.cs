using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class DeleteProjectStructureNodeEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/api/projects/{projectId}/structure/nodes/{nodeId}",
            DeleteProjectStructureNode
        );
    }

    private static async Task<NoContent> DeleteProjectStructureNode(
        [FromRoute] string projectId,
        [FromRoute] string nodeId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.NoContent();
    }
}
