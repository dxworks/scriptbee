using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Arguments;

namespace ScriptBeeWebApp.Controllers
{
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
        public IActionResult CreateProject(ProjectManagerArguments projectManagerArguments)
        {
            if (!string.IsNullOrEmpty(projectManagerArguments.projectId))
            {
                var project = _projectManager.GetProject(projectManagerArguments.projectId);

                if (project != null)
                {
                    return BadRequest($"There already is a project with the id {projectManagerArguments.projectId}");
                }

                _projectManager.AddProject(projectManagerArguments.projectId);
                return Ok("Project created successfully");
            }
            else
            {
                var id = Guid.NewGuid().ToString();
                _projectManager.AddProject(id);
                return Ok($"Project created with the id: {id}");
            }
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
}