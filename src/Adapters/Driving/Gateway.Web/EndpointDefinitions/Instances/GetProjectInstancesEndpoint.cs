using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Calculation;
using ScriptBee.Gateway.Web.EndpointDefinitions.Instances.Contracts;
using ScriptBee.Ports.Driving.UseCases.Calculation;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Instances;

public class GetProjectInstancesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetProjectInstancesUseCase, GetProjectInstancesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances", GetAllInstances);
    }

    private static async Task<Ok<WebGetProjectInstancesListResponse>> GetAllInstances(
        [FromRoute] string projectId,
        IGetProjectInstancesUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var id = ProjectId.FromValue(projectId);
        var calculationInstanceInfos = await useCase.GetAllInstances(id, cancellationToken);

        return TypedResults.Ok(WebGetProjectInstancesListResponse.Map(calculationInstanceInfos));
    }
}
