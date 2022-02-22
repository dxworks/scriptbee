using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectFileStructureManager _projectFileStructureManager;

    public ProjectsController(IProjectManager projectManager, IProjectFileStructureManager projectFileStructureManager)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
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
    public ActionResult<ReturnedProject> CreateProject(CreateProject projectArg)
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