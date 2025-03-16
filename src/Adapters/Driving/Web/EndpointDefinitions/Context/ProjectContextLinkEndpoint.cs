using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class ProjectContextLinkEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ILinkInstanceContextUseCase, LinkInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/context/link", LinkContext)
            .WithRequestValidation<WebLinkContextCommand>();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> LinkContext(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromBody] WebLinkContextCommand webCommand,
        ILinkInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var command = new LinkContextCommand(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            webCommand.LinkerIds
        );
        var result = await useCase.Link(command, cancellationToken);

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
}
