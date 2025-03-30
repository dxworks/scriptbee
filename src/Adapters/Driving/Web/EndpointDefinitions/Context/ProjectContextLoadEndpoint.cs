using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextLoadEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ILoadInstanceContextUseCase, LoadInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/context/load", LoadContext)
            .WithRequestValidation<WebLoadContextCommand>();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> LoadContext(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromBody] WebLoadContextCommand webCommand,
        ILoadInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new LoadContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            webCommand.LoaderIds
        );
        var result = await useCase.Load(command, cancellationToken);

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
