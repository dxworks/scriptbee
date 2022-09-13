using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
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
    public async Task<ActionResult<ScriptCreatedResult>> CreateScript(CreateScript arg,
        CancellationToken cancellationToken = default)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        try
        {
            var (extension, content) =
                await _projectStructureService.GetSampleCodeAsync(arg.scriptType, cancellationToken);

            if (_projectFileStructureManager.FileExists(arg.projectId, arg.filePath))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            if (!arg.filePath.EndsWith(extension))
            {
                arg.filePath += extension;
            }

            _projectFileStructureManager.CreateSrcFile(arg.projectId, arg.filePath, content);
        }
        catch
        {
            // todo have a centralized exception handler
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return new ScriptCreatedResult(arg.filePath);
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
}
