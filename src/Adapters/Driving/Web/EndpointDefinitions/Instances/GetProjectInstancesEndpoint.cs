using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Instances;

public class GetProjectInstancesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetProjectInstancesUseCase, GetProjectInstancesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances", GetAllInstances);
        app.MapGet("/api/projects/{projectId}/instances/current", GetCurrentInstance);
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

    private static async Task<Ok<WebGetProjectInstanceInfo>> GetCurrentInstance(
        [FromRoute] string projectId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebGetProjectInstanceInfo(
                "instance-id",
                ["honeydew", "InspectorGit"],
                ["software-analysis"],
                new Dictionary<string, IEnumerable<string>>
                {
                    { "InspectorGit", ["honeydew.iglog"] },
                    { "honeydew", ["honeydew-raw.json"] },
                },
                DateTimeOffset.UtcNow
            )
        );
    }
}
