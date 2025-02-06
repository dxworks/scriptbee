using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
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

    private static async Task<Created<WebCreateProjectResponse>> PostCreateProject(
        [FromBody] WebCreateProjectCommand command,
        ICreateProjectUseCase useCase, CancellationToken cancellationToken = default)
    {
        var project = await useCase.CreateProject(command.Map(), cancellationToken);

        var response = WebCreateProjectResponse.Map(project);
        return TypedResults.Created($"/api/scriptbee/projects/{response.Id}", response);
    }
}
