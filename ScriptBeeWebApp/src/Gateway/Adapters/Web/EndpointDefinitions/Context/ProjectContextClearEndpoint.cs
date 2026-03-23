using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextClearEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IClearInstanceContextUseCase, ClearInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/context/clear", ClearContext);
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
            error => error.ToProblem(context)
        );
    }
}
