using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextReloadEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IReloadInstanceContextUseCase, ReloadInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/projects/{projectId}/instances/{instanceId}/context/reload",
                ReloadContext
            )
            .WithTags("Instances", "Context")
            .WithSummary("Reload instance context")
            .WithDescription(
                "Reloads the data context for the specified project instance, refreshing all linked and loaded data."
            );
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> ReloadContext(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IReloadInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new ReloadContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId)
        );
        var result = await useCase.Reload(command, cancellationToken);

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
