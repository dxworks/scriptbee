using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class ProjectsEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/projects", GetAllProjects);
        app.MapPost("/api/projects", CreateProject);
        app.MapPut("/api/projects", UpdateProject);
        app.MapGet("/api/projects/{projectId}", GetProject);
        app.MapDelete("/api/projects/{projectId}", DeleteProject);
        app.MapGet("/api/projects/context/{projectId}", GetProjectContent);
    }

    public static async Task<IEnumerable<ReturnedProject>> GetAllProjects(IProjectModelService projectModelService,
        CancellationToken cancellationToken)
    {
        var projectModels = await projectModelService.GetAllDocuments(cancellationToken);
        return projectModels.Select(ConvertProjectModelToReturnedProject);
    }

    public static async Task<IResult> GetProject([FromRoute] string projectId, IProjectModelService projectModelService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest("You must provide a projectId for this operation");
        }

        var project = await projectModelService.GetDocument(projectId, cancellationToken);

        if (project == null)
        {
            return Results.NotFound($"Could not find project with id: {projectId}");
        }

        return Results.Ok(ConvertProjectModelToReturnedProject(project));
    }

    public static IResult GetProjectContent([FromRoute] string projectId, IProjectManager projectManager)
    {
        var project = projectManager.GetProject(projectId);

        if (project == null)
        {
            return Results.Ok(new List<string>());
        }

        var contextResult = project.Context.Models.Keys.GroupBy(tuple => tuple.Item1)
            .Select(grouping => new ReturnedContextSlice(
                grouping.Key,
                grouping.Select(tuple => tuple.Item2).ToList()
            ));

        return Results.Ok(contextResult);
    }

    public static async Task<IResult> CreateProject([FromBody] CreateProject createProject,
        IProjectManager projectManager, IProjectModelService projectModelService,
        IProjectFileStructureManager projectFileStructureManager, IValidator<CreateProject> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(createProject, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var project = projectManager.CreateProject(createProject.ProjectId, createProject.ProjectName);

        if (project == null)
        {
            return Results.BadRequest("A project with this name already exists!");
        }

        var projectModel = new ProjectModel
        {
            Id = project.Id,
            Name = project.Name,
            CreationDate = DateTime.Now
        };

        await projectModelService.CreateDocument(projectModel, cancellationToken);

        projectFileStructureManager.CreateProjectFolderStructure(createProject.ProjectId);

        return Results.Ok(projectModel);
    }

    // todo reimplement
    public static async Task<IResult> DeleteProject([FromRoute] string projectId,
        IProjectModelService projectModelService, IProjectManager projectManager, IFileModelService fileModelService,
        IRunModelService runModelService, IProjectFileStructureManager projectFileStructureManager,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest("You must provide a projectId for this operation");
        }

        var projectModel = await projectModelService.GetDocument(projectId, cancellationToken);


        if (projectModel != null)
        {
            var fileIds = new List<string>();
            foreach (var (_, savedFilesNames) in projectModel.SavedFiles)
            {
                fileIds.AddRange(savedFilesNames.Select(data => data.Id.ToString()));
            }

            await fileModelService.DeleteFilesAsync(fileIds, cancellationToken);
            await projectModelService.DeleteDocument(projectId, cancellationToken);
        }

        projectManager.RemoveProject(projectId);

        var allRunsForProject = await runModelService.GetAllRunsForProject(projectId, cancellationToken);
        foreach (var runModel in allRunsForProject)
        {
            await fileModelService.DeleteFileAsync(runModel.ScriptPath, cancellationToken);

            // todo
            foreach (var runResult in runModel.Results)
            {
                await fileModelService.DeleteFileAsync(runResult.Name, cancellationToken);
            }
        }

        projectFileStructureManager.DeleteProjectFolderStructure(projectId);

        return Results.Ok("Project removed successfully");
    }

    // todo: use or remove
    public static async Task<IResult> UpdateProject([FromBody] ProjectModel projectModel,
        IProjectManager projectManager,
        IProjectModelService projectModelService, CancellationToken cancellationToken)
    {
        var project = projectManager.GetProject(projectModel.Id);

        if (project == null)
        {
            return Results.BadRequest("Project with the given id does not exist");
        }

        await projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Results.Ok("Project removed successfully");
    }

    private static ReturnedProject ConvertProjectModelToReturnedProject(ProjectModel project)
    {
        var loadedFiles = new List<ReturnedNode>();
        var savedFiles = new List<ReturnedNode>();

        foreach (var (loaderName, files) in project.LoadedFiles)
        {
            loadedFiles.Add(new ReturnedNode(loaderName, ConvertFileNames(files)));
        }

        foreach (var (loaderName, files) in project.SavedFiles)
        {
            savedFiles.Add(new ReturnedNode(loaderName, ConvertFileNames(files)));
        }

        return new ReturnedProject
        {
            Id = project.Id,
            Name = project.Name,
            CreationDate = project.CreationDate,
            Linker = project.Linker,
            LoadedFiles = loadedFiles,
            SavedFiles = savedFiles
        };
    }

    private static List<string> ConvertFileNames(List<FileData> files)
    {
        return files.Select(f => f.Name)
            .ToList();
    }
}
