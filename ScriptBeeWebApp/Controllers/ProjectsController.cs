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

    public ProjectsController(IProjectManager projectManager, IProjectFileStructureManager projectFileStructureManager,
        IProjectModelService projectModelService)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _projectModelService = projectModelService;
    }

    [HttpGet]
    public IEnumerable<ReturnedProject> GetAllProjects()
    {
        var projects = _projectManager.GetAllProjects();

        return projects.Values.Select(project => new ReturnedProject(project.Id, project.Name, project.CreationDate));
    }

    [HttpGet("{projectId}")]
    public ActionResult<ReturnedProject> GetProject(string projectId)
    {
        if (!string.IsNullOrEmpty(projectId))
        {
            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            return new ReturnedProject(project.Id, project.Name, project.CreationDate);
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
    public async Task<ActionResult<ReturnedProject>> CreateProject(CreateProject projectArg,
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

        var projectModel = new ProjectModel();

        projectModel.Id = project.Id;
        projectModel.Name = project.Name;
        projectModel.CreationDate = DateTime.Now;

        await _projectModelService.CreateDocument(projectModel, cancellationToken);

        _projectFileStructureManager.CreateProjectFolderStructure(projectArg.projectId);

        return new ReturnedProject(project.Id, project.Name, project.CreationDate);
    }

    [HttpDelete("{projectId}")]
    public IActionResult RemoveProject(string projectId)
    {
        if (!string.IsNullOrEmpty(projectId))
        {
            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return BadRequest($"Project with the given id does not exist");
            }

            _projectManager.RemoveProject(projectId);
            return Ok("Project removed successfully");
        }

        return BadRequest("You must provide a projectId for this operation");
    }
}