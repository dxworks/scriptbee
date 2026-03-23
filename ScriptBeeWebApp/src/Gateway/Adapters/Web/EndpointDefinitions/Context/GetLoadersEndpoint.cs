using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class GetLoadersEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetInstanceLoadersUseCase, GetInstanceLoadersService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}/loaders", GetInstanceLoaders);
    }

    private static async Task<Ok<IEnumerable<WebLoader>>> GetInstanceLoaders(
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IGetInstanceLoadersUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetLoadersQuery(ProjectId.FromValue(projectId), new InstanceId(instanceId));
        var linkers = await useCase.Get(query, cancellationToken);

        return TypedResults.Ok(linkers.Select(WebLoader.Map));
    }
}
