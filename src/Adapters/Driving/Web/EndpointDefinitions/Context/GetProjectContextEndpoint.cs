using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class GetProjectContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetInstanceContextUseCase, GetInstanceContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}/context", GetCurrentContext);
    }

    private static async Task<
        Results<Ok<IEnumerable<WebProjectContextSlice>>, NotFound<ProblemDetails>>
    > GetCurrentContext(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IGetInstanceContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var query = new GetInstanceContextQuery(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId)
        );
        var result = await useCase.Get(query, cancellationToken);

        return result.Match<
            Results<Ok<IEnumerable<WebProjectContextSlice>>, NotFound<ProblemDetails>>
        >(
            slices => TypedResults.Ok(slices.Select(s => WebProjectContextSlice.Map(s))),
            error => error.ToProblem(context)
        );
    }
}
