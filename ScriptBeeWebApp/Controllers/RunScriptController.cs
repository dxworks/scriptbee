using System.Collections.Generic;
using System.Threading.Tasks;
using HelperFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Config;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptRunners;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Extensions;

namespace ScriptBeeWebApp.Controllers
{
    [ApiControllerRoute]
    [ApiController]
    public class RunScriptController : ControllerBase
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileStructureManager _projectFileStructureManager;

        public RunScriptController(IProjectManager projectManager, IHelperFunctionsMapper helperFunctionsMapper,
            IProjectFileStructureManager projectFileStructureManager)
        {
            _projectManager = projectManager;
            _helperFunctionsMapper = helperFunctionsMapper;
            _projectFileStructureManager = projectFileStructureManager;
        }

        [HttpPost("fromfile")]
        public async Task<IActionResult> RunScriptFileContent(IFormCollection formData)
        {
            if (!formData.TryGetValue("scriptType", out var scriptType))
            {
                return BadRequest("Missing script type");
            }

            var scriptRunner = GetScriptRunner(scriptType);

            if (scriptRunner == null)
            {
                return BadRequest($"Script type {scriptType} is not supported");
            }

            if (!formData.TryGetValue("projectId", out var projectId))
            {
                return BadRequest("Missing project id");
            }

            var project = _projectManager.GetProject(projectId);
            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            List<string> scriptContents = new List<string>();

            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    var scriptContent = await file.ReadFormFileContent();
                    scriptContents.Add(scriptContent);
                }
            }

            foreach (var scriptContent in scriptContents)
            {
                scriptRunner.Run(project, scriptContent);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RunScriptFromPath(RunScript arg)
        {
            if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
            {
                return BadRequest("Invalid arguments!");
            }

            var scriptType = "";
            if (arg.filePath.EndsWith(".py"))
            {
                scriptType = "python";
            }
            else if (arg.filePath.EndsWith(".cs"))
            {
                scriptType = "csharp";
            }
            else if (arg.filePath.EndsWith(".js"))
            {
                scriptType = "javascript";
            }

            var scriptRunner = GetScriptRunner(scriptType);

            if (scriptRunner == null)
            {
                return BadRequest($"Script type {scriptType} is not supported");
            }

            var project = _projectManager.GetProject(arg.projectId);
            if (project == null)
            {
                return NotFound($"Could not find project with id: {arg.projectId}");
            }

            var scriptContent = await _projectFileStructureManager.GetFileContentAsync(arg.projectId, arg.filePath);

            scriptRunner.Run(project, scriptContent);
            
            return Ok();
        }

        private IScriptRunner GetScriptRunner(string scriptType)
        {
            switch (scriptType)
            {
                case "python":
                {
                    return new PythonScriptRunner(_helperFunctionsMapper, new PythonValidScriptExtractor());
                }
                case "javascript":
                {
                    return new JavascriptScriptRunner(_helperFunctionsMapper, new JavascriptValidScriptExtractor());
                }
                case "csharp":
                {
                    return new CSharpScriptRunner(new PluginPathReader(ConfigFolders.PathToPlugins),
                        _helperFunctionsMapper);
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}