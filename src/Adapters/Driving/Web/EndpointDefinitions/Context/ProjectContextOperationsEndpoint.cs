using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextOperationsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IClearInstanceContextUseCase, ClearInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/context/clear", ClearContext);
        app.MapPost(
            "/api/projects/{projectId}/instances/{instanceId}/context/reload",
            ReloadContext
        );
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> ClearContext(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IClearInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new ClearContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId)
        );
        var result = await useCase.Clear(command, cancellationToken);

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Instance Not Found",
                        $"An instance with id '{error.InstanceId}' is not allocated."
                    )
                )
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
