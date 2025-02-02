using ScriptBee.Common.Web;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project;

public class CreateProject : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/projects", PostCreateProject)
            .RequireAuthorization("create_project");
    }

    private static async Task<IResult> PostCreateProject(
        // [FromBody] CreateProject createProject,
        // IProjectManager projectManager, IProjectModelService projectModelService,
        // IProjectFileStructureManager projectFileStructureManager, IValidator<CreateProject> validator,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return Results.Ok();
    }

    public record WebCreateProjectCommand(string Name);
}
