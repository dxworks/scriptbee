using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Services;
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
    public ActionResult<ScriptCreatedResult> CreateScript(CreateScript arg)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        try
        {
            var content = "";
            // todo
            // switch (arg.scriptType)
            // {
            //     case "python":
            //     {
            //         if (!arg.filePath.EndsWith(".py"))
            //         {
            //             arg.filePath += ".py";
            //         }
            //
            //         content = new SampleCodeGenerator(new PythonScriptGeneratorStrategy(new RelativeFileContentProvider()),
            //             _loadersHolder).GenerateSampleCode();
            //     }
            //         break;
            //     case "csharp":
            //     {
            //         if (!arg.filePath.EndsWith(".cs"))
            //         {
            //             arg.filePath += ".cs";
            //         }
            //
            //         content = new SampleCodeGenerator(new CSharpScriptGeneratorStrategy(new RelativeFileContentProvider()),
            //             _loadersHolder).GenerateSampleCode();
            //     }
            //         break;
            //     case "javascript":
            //     {
            //         if (!arg.filePath.EndsWith(".js"))
            //         {
            //             arg.filePath += ".js";
            //         }
            //
            //         content = new SampleCodeGenerator(
            //             new JavascriptScriptGeneratorStrategy(new RelativeFileContentProvider()),
            //             _loadersHolder).GenerateSampleCode();
            //     }
            //         break;
            // }

            if (_projectFileStructureManager.FileExists(arg.projectId, arg.filePath))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            _projectFileStructureManager.CreateSrcFile(arg.projectId, arg.filePath, content);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return new ScriptCreatedResult(arg.filePath);
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

    [HttpGet("scriptabsolutepath")]
    public ActionResult<string> GetScriptAbsolutePath([FromQuery] GetScriptDetails arg)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        return _projectFileStructureManager.GetAbsoluteFilePath(arg.projectId, arg.filePath);
    }

    [HttpGet("projectabsolutepath")]
    public ActionResult<string> GetProjectAbsolutePath([FromQuery] string projectId)
    {
        if (projectId == null || string.IsNullOrEmpty(projectId))
        {
            return BadRequest("Invalid arguments!");
        }

        return _projectFileStructureManager.GetProjectAbsolutePath(projectId);
    }
}
