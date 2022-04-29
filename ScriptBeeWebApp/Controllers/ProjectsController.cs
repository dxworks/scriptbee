using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Models;
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

    public ProjectsController(IProjectManager projectManager, IProjectFileStructureManager projectFileStructureManager,
        IProjectModelService projectModelService, IFileNameGenerator fileNameGenerator)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _projectModelService = projectModelService;
        _fileNameGenerator = fileNameGenerator;
    }

    [HttpGet]
    public async Task<IEnumerable<ReturnedProject>> GetAllProjects(CancellationToken cancellationToken)
    {
        var projectModels = await _projectModelService.GetAllDocuments(cancellationToken);
        return projectModels.Select(ConvertProjectModelToReturnedProject);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ReturnedNode>> GetProject(string projectId, CancellationToken cancellationToken)
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
        if (!string.IsNullOrEmpty(projectId))
        {
            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return BadRequest($"Project with the given id does not exist");
            }

            _projectManager.RemoveProject(projectId);

            await _projectModelService.DeleteDocument(projectId, cancellationToken);

            return Ok("Project removed successfully");
        }

        return BadRequest("You must provide a projectId for this operation");
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
            Loaders = project.Loaders,
            LoadedFiles = loadedFiles,
            SavedFiles = savedFiles
        };
    }

    private List<string> ConvertFileNames(IEnumerable<string> files)
    {
        return files.Select(file =>
        {
            var (_, fileName, _) = _fileNameGenerator.ExtractModelNameComponents(file);
            return fileName;
        }).ToList();
    }
}