using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Service.Project;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Project;

public class CreateProjectEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ICreateProjectUseCase, CreateProjectService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects", CreateProject)
            .WithRequestValidation<WebCreateProjectCommand>();
    }

    private static async Task<
        Results<Created<WebCreateProjectResponse>, Conflict<ProblemDetails>>
    > CreateProject(
        HttpContext context,
        [FromBody] WebCreateProjectCommand command,
        ICreateProjectUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.CreateProject(command.Map(), cancellationToken);

        return result.Match<Results<Created<WebCreateProjectResponse>, Conflict<ProblemDetails>>>(
            projectDetails =>
            {
                var response = WebCreateProjectResponse.Map(projectDetails);
                return TypedResults.Created($"/api/projects/{response.Id}", response);
            },
            error =>
                TypedResults.Conflict(
                    context.ToProblemDetails(
                        "Project ID Already In Use",
                        $"A project with the ID '{error.Id.Value}' already exists. Use a unique Project ID or update the existing project."
                    )
                )
        );
    }
}
