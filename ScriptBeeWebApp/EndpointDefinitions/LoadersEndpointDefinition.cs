using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class LoadersEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/loaders", GetAllProjectLoaders);
        app.MapPost("/api/loaders", LoadFiles);
        app.MapPost("/api/loaders/{projectId}", ReloadProjectContext);
        app.MapPost("/api/loaders/clear/{projectId}", ClearProjectContext);
    }

    public static IEnumerable<string> GetAllProjectLoaders(ILoadersService loadersService)
    {
        return loadersService.GetSupportedLoaders();
    }

    public static async Task<IResult> LoadFiles([FromBody] LoadModels loadModels, IValidator<LoadModels> validator,
        ILoadersService loadersService, IProjectManager projectManager, IProjectModelService projectModelService,
        IProjectStructureService projectStructureService, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(loadModels, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        // todo maybe move into a validator and use di for it to get LoaderService
        foreach (var (loader, _) in loadModels.Nodes)
        {
            var modelLoader = loadersService.GetLoader(loader);

            if (modelLoader == null)
            {
                return Results.BadRequest($"Model type {loader} is not supported");
            }
        }

        var projectModel = await projectModelService.GetDocument(loadModels.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {loadModels.ProjectId}");
        }

        var project = projectManager.GetProject(loadModels.ProjectId);

        if (project == null)
        {
            projectManager.LoadProject(projectModel);
        }

        var loadedModels = await loadersService.LoadFiles(projectModel, loadModels.Nodes, cancellationToken);

        await projectStructureService.GenerateModelClasses(loadModels.ProjectId, cancellationToken);

        return Results.Ok(ConvertLoadedModels(loadedModels));
    }

    // todo extract validation to separate class
    public static async Task<IResult> ReloadProjectContext([FromRoute] string projectId,
        IProjectModelService projectModelService, ILoadersService loadersService, IProjectManager projectManager,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            return Results.BadRequest("Invalid argument. ProjectId needed!");
        }

        var projectModel = await projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {projectId}");
        }

        if (projectModel.LoadedFiles.Count == 0)
        {
            return Results.Ok();
        }

        // todo maybe move into a validator and use di for it to get LoaderService
        foreach (var (loader, _) in projectModel.LoadedFiles)
        {
            var modelLoader = loadersService.GetLoader(loader);

            if (modelLoader == null)
            {
                return Results.BadRequest($"Model type {loader} is not supported");
            }
        }

        var project = projectManager.GetProject(projectId);
        if (project == null)
        {
            projectManager.LoadProject(projectModel);
        }

        var loadedModels = await loadersService.ReloadModels(projectModel, cancellationToken);

        return Results.Ok(ConvertLoadedModels(loadedModels));
    }

    // todo extract validation to separate class
    public static async Task<IResult> ClearProjectContext([FromRoute] string projectId,
        IProjectModelService projectModelService, IProjectManager projectManager, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            return Results.BadRequest("Invalid argument. ProjectId needed!");
        }

        var projectModel = await projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {projectId}");
        }

        // todo extract in loaders service
        var project = projectManager.GetProject(projectId);
        project?.Context.Clear();

        projectModel.LoadedFiles.Clear();

        await projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Results.Ok();
    }

    private static IEnumerable<ReturnedContextSlice> ConvertLoadedModels(Dictionary<string, List<string>> loadedModels)
    {
        return loadedModels.Select(x => new ReturnedContextSlice(
            x.Key, x.Value));
    }
}
