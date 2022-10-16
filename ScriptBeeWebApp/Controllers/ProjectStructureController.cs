using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class ProjectStructureController : ControllerBase
{
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IProjectStructureService _projectStructureService;

    public ProjectStructureController(IProjectFileStructureManager projectFileStructureManager,
        IProjectStructureService projectStructureService)
    {
        _projectFileStructureManager = projectFileStructureManager;
        _projectStructureService = projectStructureService;
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
    // todo extract validation in a separate class
    public async Task<ActionResult<FileTreeNode>> CreateScript(CreateScript createScript,
        CancellationToken cancellationToken = default)
    {
        if (createScript == null || string.IsNullOrEmpty(createScript.projectId) ||
            string.IsNullOrEmpty(createScript.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        try
        {
            var (extension, content) =
                await _projectStructureService.GetSampleCodeAsync(createScript.scriptType, cancellationToken);

            if (!createScript.filePath.EndsWith(extension))
            {
                createScript.filePath += extension;
            }

            if (_projectFileStructureManager.FileExists(createScript.projectId, createScript.filePath))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            var node = _projectFileStructureManager.CreateSrcFile(createScript.projectId, createScript.filePath,
                content);

            return Ok(node);
        }
        catch
        {
            // todo have a centralized exception handler
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("script")]
    // todo extract validation om a separate class
    public async Task<ActionResult<string>> GetScriptContent([FromQuery] GetScriptDetails? arg)
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
    // todo extract validation om a separate class
    public ActionResult<string> GetScriptAbsolutePath([FromQuery] GetScriptDetails? arg)
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
        if (string.IsNullOrEmpty(projectId))
        {
            return BadRequest("Invalid arguments!");
        }

        return _projectFileStructureManager.GetProjectAbsolutePath(projectId);
    }

    [HttpPost("filewatcher")]
    // todo add validation
    public IActionResult SetupFileWatcher([FromBody] SetupFileWatcher setupFileWatcher)
    {
        _projectFileStructureManager.SetupFileWatcher(setupFileWatcher.ProjectId, setupFileWatcher.FilePath);

        // todo return something
        return Ok("");
    }
}
