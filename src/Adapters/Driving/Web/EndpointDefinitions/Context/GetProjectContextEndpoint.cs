using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class GetProjectContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}/context", GetCurrentContext);
    }

    private static async Task<Ok<IEnumerable<WebGetProjectContextResponse>>> GetCurrentContext(
        [FromRoute] string projectId,
        [FromRoute] string instanceId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            (IEnumerable<WebGetProjectContextResponse>)
                [
                    new WebGetProjectContextResponse("Repository", ["InspectorGit", "honeydew"]),
                    new WebGetProjectContextResponse("Class", ["honeydew"]),
                ]
        );
    }
}
