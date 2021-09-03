using System.Collections.Generic;
using System.Threading.Tasks;
using HelperFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptRunners;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Extensions;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunScriptController : ControllerBase
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;

        private readonly IProjectManager _projectManager;

        public RunScriptController(IProjectManager projectManager, IHelperFunctionsMapper helperFunctionsMapper)
        {
            _projectManager = projectManager;
            _helperFunctionsMapper = helperFunctionsMapper;
        }

        [HttpPost("fromfile")]
        public async Task<IActionResult> PostFromFile(IFormCollection formData)
        {
            if (!formData.TryGetValue("scriptType", out var scriptType))
            {
                return BadRequest("Missing script type");
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

            var scriptRunner = GetScriptRunner(scriptType);

            if (scriptRunner == null)
            {
                return BadRequest($"Script type {scriptType} is not supported");
            }

            var project = _projectManager.GetProject();

            foreach (var scriptContent in scriptContents)
            {
                scriptRunner.Run(project, scriptContent);
            }

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
                // case "csharp":
                // {
                //     return new CSharpDummyScriptRunner();
                // }
                default:
                {
                    return null;
                }
            }
        }
    }
}