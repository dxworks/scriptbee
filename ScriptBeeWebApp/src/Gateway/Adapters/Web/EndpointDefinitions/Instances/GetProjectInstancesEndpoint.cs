using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Instances;

public class GetProjectInstancesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetProjectInstancesUseCase, GetProjectInstancesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances", GetAllInstances)
            .WithTags("Instances")
            .WithSummary("Get all project instances")
            .WithDescription(
                "Retrieves a list of all instances associated with the specified project."
            );
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}", GetInstance)
            .WithTags("Instances")
            .WithSummary("Get project instance by ID")
            .WithDescription("Retrieves detailed information about a specific project instance.");
    }

    private static async Task<Ok<WebGetProjectInstancesListResponse>> GetAllInstances(
        [FromRoute] string projectId,
        IGetProjectInstancesUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var id = ProjectId.FromValue(projectId);
        var instanceInfos = await useCase.GetAllInstances(id, cancellationToken);

        return TypedResults.Ok(WebGetProjectInstancesListResponse.Map(instanceInfos));
    }

    private static async Task<
        Results<Ok<WebProjectInstance>, NotFound<ProblemDetails>>
    > GetInstance(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        IGetProjectInstancesUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetInstance(
            ProjectId.FromValue(projectId),
            new InstanceId(instanceId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebProjectInstance>, NotFound<ProblemDetails>>>(
            info => TypedResults.Ok(WebProjectInstance.Map(info)),
            error => error.ToProblem(context)
        );
    }
}
