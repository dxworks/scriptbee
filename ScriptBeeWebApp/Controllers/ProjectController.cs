﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Arguments;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectManager _projectManager;

    public ProjectController(IProjectManager projectManager)
    {
        _projectManager = projectManager;
    }

    [HttpPost("create")]
    public IActionResult CreateProject(CreateProject projectArg)
    {
        if (projectArg == null || string.IsNullOrWhiteSpace(projectArg.projectName))
        {
            return BadRequest("Invalid arguments. Creation failed!");
        }

        var project = _projectManager.CreateProject(projectArg.projectName);
        return Ok(project);
    }

    [HttpPost("remove/{projectId}")]
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

    [HttpGet("get/{projectId}")]
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

    [HttpGet("getAll")]
    public IActionResult GetAllProjects()
    {
        var projects = _projectManager.GetAllProjects();

        return Ok(projects.Values.ToList());
    }
}