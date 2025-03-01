using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextOperationsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/context/clear", ClearContext);
        app.MapPost(
            "/api/projects/{projectId}/instances/{instanceId}/context/reload",
            ReloadContext
        );
    }

    private static async Task<NoContent> ClearContext(
        [FromRoute] string projectId,
        [FromRoute] string instanceId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: implement

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> ReloadContext(
        [FromRoute] string projectId,
        [FromRoute] string instanceId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: implement

        return TypedResults.NoContent();
    }
}
