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
        app.MapPost("/api/projectstructure/script", CreateScript);
        app.MapGet("/api/projectstructure/scriptabsolutepath", GetScriptAbsolutePath);
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

    public static async Task<IResult> CreateScript([FromBody] CreateScript createScript,
        IValidator<CreateScript> validator, IProjectFileStructureManager projectFileStructureManager,
        IProjectStructureService projectStructureService,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(createScript, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var (extension, content) =
            await projectStructureService.GetSampleCodeAsync(createScript.ScriptLanguage, cancellationToken);

        if (!createScript.FilePath.EndsWith(extension))
        {
            createScript.FilePath += extension;
        }

        if (projectFileStructureManager.FileExists(createScript.ProjectId, createScript.FilePath))
        {
            return Results.Conflict();
        }

        var node = projectFileStructureManager.CreateSrcFile(createScript.ProjectId, createScript.FilePath,
            content);

        return Results.Ok(node);
    }

    public static IResult GetScriptAbsolutePath([FromQuery] string projectId, [FromQuery] string filePath,
        IProjectFileStructureManager projectFileStructureManager, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(filePath))
        {
            return Results.BadRequest("Invalid arguments!");
        }

        return Results.Ok(projectFileStructureManager.GetAbsoluteFilePath(projectId, filePath));
    }

    public static IResult GetProjectAbsolutePath([FromQuery] string projectId,
        IProjectFileStructureManager projectFileStructureManager)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest("Invalid arguments!");
        }

        return Results.Ok(projectFileStructureManager.GetProjectAbsolutePath(projectId));
    }

    public static async Task<IResult> SetupFileWatcher([FromBody] SetupFileWatcher setupFileWatcher,
        IValidator<SetupFileWatcher> validator, IProjectFileStructureManager projectFileStructureManager,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(setupFileWatcher, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        projectFileStructureManager.SetupFileWatcher(setupFileWatcher.ProjectId);

        // todo return something
        return Results.Ok("");
    }

    public static IResult RemoveFileWatcher(string projectId, IProjectFileStructureManager projectFileStructureManager)
    {
        projectFileStructureManager.RemoveFileWatcher(projectId);

        // todo return something
        return Results.Ok("");
    }
}
