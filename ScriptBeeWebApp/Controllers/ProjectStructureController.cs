using System.Threading.Tasks;
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
                    content = new SampleCodeGenerator(new PythonStrategyGenerator(new FileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
                case "csharp":
                {
                    content = new SampleCodeGenerator(new CSharpStrategyGenerator(new FileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
                case "javascript":
                {
                    content = new SampleCodeGenerator(new JavascriptStrategyGenerator(new FileContentProvider()),
                        _loadersHolder).GenerateSampleCode();
                }
                    break;
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
    public async Task<ActionResult<string>> GetScriptContent(GetScriptContent arg)
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
}