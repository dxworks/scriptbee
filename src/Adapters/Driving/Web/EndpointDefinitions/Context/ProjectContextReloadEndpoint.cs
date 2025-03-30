using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
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
            error =>error.ToProblem(context)
        );
    }
}
