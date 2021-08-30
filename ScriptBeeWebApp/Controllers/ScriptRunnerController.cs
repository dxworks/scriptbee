using HelperFunctions;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models.Dummy;
using ScriptBee.Plugins;
using ScriptBee.Scripts.ScriptRunners;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScriptRunnerController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(ScriptRunnerArguments scriptRunnerArguments)
        {
            HelperFunctionsMapper helperFunctionsMapper = new HelperFunctionsMapper();

            switch (scriptRunnerArguments.ModelType)
            {
                case "dummy":
                {
                    DummyModelLoader dummyModelLoader = new DummyModelLoader();
                    var loadedModel = dummyModelLoader.LoadModel(scriptRunnerArguments.ModelJsonContent);
                    
                    switch (scriptRunnerArguments.ScriptType)
                    {
                        case "python":
                        {
                            var pythonDummyScriptRunner = new PythonDummyScriptRunner(helperFunctionsMapper);
                            pythonDummyScriptRunner.RunScript(loadedModel,scriptRunnerArguments.ScriptContent);
                            return Ok(scriptRunnerArguments);
                        }
                        case "javascript":
                        {
                            var javascriptDummyScriptRunner = new JavascriptDummyScriptRunner(helperFunctionsMapper);
                            javascriptDummyScriptRunner.RunScript(loadedModel,scriptRunnerArguments.ScriptContent);
                            return Ok(scriptRunnerArguments);
                        }
                        case "csharp":
                        {
                            // var cSharpDummyScriptRunner = new CSharpDummyScriptRunner(new PluginLoader(""));
                            // cSharpDummyScriptRunner.RunScript(loadedModel,scriptRunnerArguments.ScriptContent);
                            // return Ok(scriptRunnerArguments);
                            return BadRequest("Not implemented");
                        }
                        default:
                        {
                            return BadRequest($"Script type {scriptRunnerArguments.ScriptType} is not supported");
                        }
                    }
                }
                default:
                {
                    return BadRequest($"Model type {scriptRunnerArguments.ModelType} is not supported");
                }
            }
        }
    }
}