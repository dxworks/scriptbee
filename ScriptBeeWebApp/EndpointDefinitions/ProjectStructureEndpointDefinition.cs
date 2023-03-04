using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class ProjectStructureEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IConfigFoldersService, ConfigFoldersService>();
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/projectstructure/{projectId}", GetProjectStructure);
        app.MapGet("/api/projectstructure/projectabsolutepath", GetProjectAbsolutePath);
        app.MapPost("/api/projectstructure/filewatcher", SetupFileWatcher);
        app.MapDelete("/api/projectstructure/filewatcher/{projectId}", RemoveFileWatcher);
    }

    public static IResult GetProjectStructure([FromRoute] string projectId,
        IProjectFileStructureManager projectFileStructureManager)
    {
        var fileTreeNode = projectFileStructureManager.GetSrcStructure(projectId);

        if (fileTreeNode == null)
        {
            return Results.BadRequest("Project with given id does not exist");
        }

        return Results.Ok(fileTreeNode);
    }

    private static IResult GetProjectAbsolutePath([FromQuery] string projectId,
        IProjectFileStructureManager projectFileStructureManager)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest("Invalid arguments!");
        }

        return Results.Ok(projectFileStructureManager.GetProjectAbsolutePath(projectId));
    }

    private static async Task<IResult> SetupFileWatcher([FromBody] SetupFileWatcher setupFileWatcher,
        IValidator<SetupFileWatcher> validator, IProjectFileStructureManager projectFileStructureManager,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(setupFileWatcher, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        projectFileStructureManager.SetupFileWatcher(setupFileWatcher.ProjectId);

        return Results.Ok();
    }

    private static IResult RemoveFileWatcher(string projectId, IProjectFileStructureManager projectFileStructureManager)
    {
        projectFileStructureManager.RemoveFileWatcher(projectId);

        return Results.Ok();
    }
}
