﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class ProjectStructureController : ControllerBase
{
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly ILoadersHolder _loadersHolder;

    public ProjectStructureController(IProjectFileStructureManager projectFileStructureManager,
        ILoadersHolder loadersHolder)
    {
        _projectFileStructureManager = projectFileStructureManager;
        _loadersHolder = loadersHolder;
    }

    [HttpGet("{projectId}")]
    public ActionResult<FileTreeNode> GetProjectStructure(string projectId)
    {
        var fileTreeNode = _projectFileStructureManager.GetSrcStructure(projectId);

        if (fileTreeNode == null)
        {
            return BadRequest("Project with given id does not exist");
        }

        return fileTreeNode;
    }

    [HttpPost("script")]
    public IActionResult CreateScript(CreateScript arg)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        try
        {
            var content = "";
            switch (arg.scriptType)
            {
                case "python":
                {
                    if (!arg.filePath.EndsWith(".py"))
                    {
                        arg.filePath += ".py";
                    }

                    content = new SampleCodeGenerator(new PythonStrategyGenerator(new RelativeFileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
                case "csharp":
                {
                    if (!arg.filePath.EndsWith(".cs"))
                    {
                        arg.filePath += ".cs";
                    }

                    content = new SampleCodeGenerator(new CSharpStrategyGenerator(new RelativeFileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
                case "javascript":
                {
                    if (!arg.filePath.EndsWith(".js"))
                    {
                        arg.filePath += ".js";
                    }

                    content = new SampleCodeGenerator(
                        new JavascriptStrategyGenerator(new RelativeFileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
            }

            if (_projectFileStructureManager.FileExists(arg.projectId, arg.filePath))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            _projectFileStructureManager.CreateFile(arg.projectId, arg.filePath, content);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok("Script created successfully");
    }

    [HttpGet("script")]
    public async Task<ActionResult<string>> GetScriptContent([FromQuery] GetScriptDetails arg)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        var content = await _projectFileStructureManager.GetFileContentAsync(arg.projectId, arg.filePath);

        if (content == null)
        {
            return NotFound("Script not found");
        }

        return Ok(content);
    }

    [HttpGet("absolutepath")]
    public ActionResult<string> GetScriptAbsolutePath([FromQuery] GetScriptDetails arg)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        return _projectFileStructureManager.GetAbsoluteFilePath(arg.projectId, arg.filePath);
    }
}