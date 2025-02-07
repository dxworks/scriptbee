using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Problems;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Service.Projects;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Gateway.Web.Extensions;
using ScriptBee.Ports.Driving.UseCases.Projects;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project;

public class CreateProject : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ICreateProjectUseCase, CreateProjectService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/scriptbee/projects", PostCreateProject)
            .RequireAuthorization(AuthorizationRolesExtensions.CreateProjectPolicy)
            .WithRequestValidation<WebCreateProjectCommand>();
    }

    private static async Task<IResult> PostCreateProject(
        [FromBody] WebCreateProjectCommand command,
        ICreateProjectUseCase useCase, CancellationToken cancellationToken = default)
    {
        var result = await useCase.CreateProject(command.Map(), cancellationToken);

        return result.Match<IResult>(
            projectDetails =>
            {
                var response = WebCreateProjectResponse.Map(projectDetails);
                return TypedResults.Created($"/api/scriptbee/projects/{response.Id}", response);
            },
            error => ProblemsFactory.Conflict(
                "Project ID Already In Use",
                $"A project with the ID '{error.Id.Value}' already exists. Use a unique Project ID or update the existing project."
            )
        );
    }
}
