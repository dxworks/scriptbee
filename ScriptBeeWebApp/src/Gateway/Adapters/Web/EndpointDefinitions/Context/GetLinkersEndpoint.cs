using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class GetLinkersEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetInstanceLinkersUseCase, GetInstanceLinkersService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}/linkers", GetInstanceLinkers)
            .WithTags("Instances", "Context");
    }

    private static async Task<Ok<WebGetLinkersResponse>> GetInstanceLinkers(
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IGetInstanceLinkersUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetLinkersQuery(ProjectId.FromValue(projectId), new InstanceId(instanceId));
        var linkers = await useCase.Get(query, cancellationToken);

        return TypedResults.Ok(new WebGetLinkersResponse(linkers.Select(WebLinker.Map)));
    }
}
