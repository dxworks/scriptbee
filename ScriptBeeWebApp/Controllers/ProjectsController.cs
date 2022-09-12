using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileModelService _fileModelService;
    private readonly IRunModelService _runModelService;

    public ProjectsController(IProjectManager projectManager, IProjectFileStructureManager projectFileStructureManager,
        IProjectModelService projectModelService, IFileNameGenerator fileNameGenerator,
        IFileModelService fileModelService, IRunModelService runModelService)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _projectModelService = projectModelService;
        _fileNameGenerator = fileNameGenerator;
        _fileModelService = fileModelService;
        _runModelService = runModelService;
    }

    [HttpGet]
    public async Task<IEnumerable<ReturnedProject>> GetAllProjects(CancellationToken cancellationToken)
    {
        var projectModels = await _projectModelService.GetAllDocuments(cancellationToken);
        return projectModels.Select(ConvertProjectModelToReturnedProject);
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(string projectId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(projectId))
        {
            var project = await _projectModelService.GetDocument(projectId, cancellationToken);

            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            return Ok(ConvertProjectModelToReturnedProject(project));
        }

        return BadRequest("You must provide a projectId for this operation");
    }

    [HttpGet("context/{projectId}")]
    public IActionResult GetProjectContent(string projectId)
    {
        var project = _projectManager.GetProject(projectId);

        if (project == null)
        {
            return Ok(new List<string>());
        }

        var contextResult = project.Context.Models.Keys.GroupBy(tuple => tuple.Item1)
            .Select(grouping => new
            {
                name = grouping.Key,
                children = grouping.Select(t => new
                {
                    name = t.Item2
                })
            });

        return Ok(contextResult);
    }

    [HttpPost]
    // todo extract validation to separate class
    public async Task<ActionResult<ProjectModel>> CreateProject(CreateProject projectArg,
        CancellationToken cancellationToken)
    {
        if (projectArg == null || string.IsNullOrWhiteSpace(projectArg.projectId) ||
            string.IsNullOrEmpty(projectArg.projectName))
        {
            return BadRequest("Invalid arguments. Creation failed!");
        }

        var project = _projectManager.CreateProject(projectArg.projectId, projectArg.projectName);

        if (project == null)
        {
            return BadRequest("A project with this name already exists!");
        }

        var projectModel = new ProjectModel
        {
            Id = project.Id,
            Name = project.Name,
            CreationDate = DateTime.Now,
        };

        await _projectModelService.CreateDocument(projectModel, cancellationToken);

        _projectFileStructureManager.CreateProjectFolderStructure(projectArg.projectId);

        return projectModel;
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> RemoveProject(string projectId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return BadRequest("You must provide a projectId for this operation");
        }

        var projectModel = await _projectModelService.GetDocument(projectId, cancellationToken);

        if (projectModel != null)
        {
            foreach (var (_, savedFilesNames) in projectModel.SavedFiles)
            {
                foreach (var fileName in savedFilesNames)
                {
                    await _fileModelService.DeleteFileAsync(fileName, cancellationToken);
                }
            }

            await _projectModelService.DeleteDocument(projectId, cancellationToken);
        }

        _projectManager.RemoveProject(projectId);

        var allRunsForProject = await _runModelService.GetAllRunsForProject(projectId, cancellationToken);
        foreach (var runModel in allRunsForProject)
        {
            await _fileModelService.DeleteFileAsync(runModel.ScriptName, cancellationToken);

            await _fileModelService.DeleteFileAsync(runModel.ConsoleOutputName, cancellationToken);

            foreach (var outputFileName in runModel.OutputFileNames)
            {
                await _fileModelService.DeleteFileAsync(outputFileName, cancellationToken);
            }

            await _runModelService.DeleteDocument(runModel.Id, cancellationToken);
        }

        return Ok("Project removed successfully");
    }

    [HttpPut("{projectModel}")]
    public async Task<IActionResult> UpdateProject(ProjectModel projectModel, CancellationToken cancellationToken)
    {
        var project = _projectManager.GetProject(projectModel.Id);

        if (project == null)
        {
            return BadRequest($"Project with the given id does not exist");
        }

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok("Project removed successfully");
    }

    private ReturnedProject ConvertProjectModelToReturnedProject(ProjectModel project)
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

    private List<string> ConvertFileNames(IEnumerable<string> files)
    {
        return files.Select(file =>
        {
            var (_, _, fileName) = _fileNameGenerator.ExtractModelNameComponents(file);
            return fileName;
        }).ToList();
    }
}
