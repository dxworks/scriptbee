using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectManager _projectManager;

    public ProjectsController(IProjectManager projectManager)
    {
        _projectManager = projectManager;
    }

    [HttpGet]
    public IActionResult GetAllProjects()
    {
        var projects = _projectManager.GetAllProjects();

        return Ok(projects.Values.ToList());
    }

    [HttpGet("{projectId}")]
    public IActionResult GetProject(string projectId)
    {
        if (!string.IsNullOrEmpty(projectId))
        {
            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            return Ok(JsonConvert.SerializeObject(project, Formatting.Indented));
        }

        return BadRequest("You must provide a projectId for this operation");
    }

    [HttpGet("/content/{projectId}")]
    public IActionResult GetProjectContent(string projectId)
    {
        var project = _projectManager.GetProject(projectId);

        var dictionary = project.Context.Keys.GroupBy(tuple => tuple.Item1)
            .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(t => t.Item2).ToList());

        return Ok(dictionary);
    }

    [HttpPost]
    public IActionResult CreateProject(CreateProject projectArg)
    {
        if (projectArg == null || string.IsNullOrWhiteSpace(projectArg.projectName))
        {
            return BadRequest("Invalid arguments. Creation failed!");
        }

        var project = _projectManager.CreateProject(projectArg.projectName);
        return Ok(project);
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