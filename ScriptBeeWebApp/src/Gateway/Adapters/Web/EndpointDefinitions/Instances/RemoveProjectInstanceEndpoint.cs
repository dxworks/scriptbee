using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Instances;

public class RemoveProjectInstanceEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<
            IDeallocateProjectInstanceUseCase,
            DeallocateProjectInstanceService
        >();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}/instances/{instanceId}", DeallocateInstance)
            .WithTags("Instances");
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeallocateInstance(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IDeallocateProjectInstanceUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.Deallocate(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            cancellationToken
        );

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context)
        );
    }
}
