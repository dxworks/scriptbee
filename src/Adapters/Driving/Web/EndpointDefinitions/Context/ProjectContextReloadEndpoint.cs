using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextReloadEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IClearInstanceContextUseCase, ClearInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/projects/{projectId}/instances/{instanceId}/context/reload",
            ReloadContext
        );
    }

    private static async Task<NoContent> ReloadContext(
        [FromRoute] string projectId,
        [FromRoute] string instanceId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT(#47): implement

        return TypedResults.NoContent();
    }
}
