using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
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
        app.MapGet("/api/projects", GetAllProjects)
            .WithTags("Projects")
            .WithSummary("Get all projects")
            .WithDescription("Retrieves a list of all existing projects with their basic details.");
        app.MapGet("/api/projects/{projectId}", GetProjectById)
            .WithTags("Projects")
            .WithSummary("Get project by ID")
            .WithDescription(
                "Retrieves detailed information about a specific project by its unique identifier."
            );
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
        Results<Ok<WebProjectDetails>, NotFound<ProblemDetails>>
    > GetProjectById(
        HttpContext context,
        [FromRoute] string projectId,
        IGetProjectsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetProjectQuery(ProjectId.FromValue(projectId));
        var result = await useCase.GetProject(query, cancellationToken);

        return result.Match<Results<Ok<WebProjectDetails>, NotFound<ProblemDetails>>>(
            projectDetails => TypedResults.Ok(WebProjectDetails.Map(projectDetails)),
            error => error.ToProblem(context)
        );
    }
}
