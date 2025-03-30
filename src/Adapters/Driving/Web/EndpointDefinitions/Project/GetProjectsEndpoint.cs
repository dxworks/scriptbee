using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Project;

public class GetProjectsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetProjectsUseCase, GetProjectsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects", GetAllProjects);
        app.MapGet("/api/projects/{projectId}", GetProjectById);
    }

    private static async Task<Ok<WebGetProjectListResponse>> GetAllProjects(
        IGetProjectsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetAllProjects(cancellationToken);

        return TypedResults.Ok(WebGetProjectListResponse.Map(result));
    }

    private static async Task<
        Results<Ok<WebGetProjectDetailsResponse>, NotFound<ProblemDetails>>
    > GetProjectById(
        HttpContext context,
        [FromRoute] string projectId,
        IGetProjectsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetProjectQuery(ProjectId.FromValue(projectId));
        var result = await useCase.GetProject(query, cancellationToken);

        return result.Match<Results<Ok<WebGetProjectDetailsResponse>, NotFound<ProblemDetails>>>(
            projectDetails => TypedResults.Ok(WebGetProjectDetailsResponse.Map(projectDetails)),
            error => error.ToProblem(context)
        );
    }
}
