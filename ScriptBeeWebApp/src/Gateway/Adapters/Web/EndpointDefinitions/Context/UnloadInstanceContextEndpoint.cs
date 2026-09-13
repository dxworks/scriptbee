using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

using UnloadResult = Results<NoContent, NotFound<ProblemDetails>>;

public class UnloadInstanceContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUnloadInstanceContextUseCase, UnloadInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/api/projects/{projectId}/instances/{instanceId}/context/loaders/{loaderId}/files/{fileId}",
                UnloadFile
            )
            .WithTags("Instances", "Context")
            .WithSummary("Unload a model file from instance context")
            .WithDescription(
                "Unloads a specific model file for a loader from the instance context and project state."
            )
            .RequireAction("model:load");

        app.MapDelete(
                "/api/projects/{projectId}/instances/{instanceId}/context/loaders/{loaderId}",
                UnloadLoader
            )
            .WithTags("Instances", "Context")
            .WithSummary("Unload all model files for a loader from instance context")
            .WithDescription(
                "Unloads all model files for a specific loader from the instance context and project state."
            )
            .RequireAction("model:load");
    }

    private static async Task<UnloadResult> UnloadFile(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromRoute] string loaderId,
        [FromRoute] string fileId,
        IUnloadInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new UnloadInstanceContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            loaderId,
            new FileId(fileId)
        );

        var result = await useCase.Unload(command, cancellationToken);

        return result.Match<UnloadResult>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }

    private static async Task<UnloadResult> UnloadLoader(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromRoute] string loaderId,
        IUnloadInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new UnloadInstanceContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            loaderId
        );

        var result = await useCase.Unload(command, cancellationToken);

        return result.Match<UnloadResult>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
