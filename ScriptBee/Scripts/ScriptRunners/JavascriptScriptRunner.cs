using System.Collections.Generic;
using System.Threading.Tasks;
using HelperFunctions;
using Jint;
using ScriptBee.ProjectContext;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Services;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class JavascriptScriptRunner : IScriptRunner
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;
        private readonly IHelperFunctionsFactory _helperFunctionsFactory;
        private readonly ValidScriptExtractor _scriptExtractor;


        public JavascriptScriptRunner(ValidScriptExtractor scriptExtractor,
            IHelperFunctionsFactory helperFunctionsFactory,
            IHelperFunctionsMapper helperFunctionsMapper)
        {
            _helperFunctionsMapper = helperFunctionsMapper;
            _scriptExtractor = scriptExtractor;
            _helperFunctionsFactory = helperFunctionsFactory;
        }

        public Task<List<RunResult>> Run(Project project, string runId, string scriptContent)
        {
            var engine = new Engine();
            engine.SetValue("project", project);

            var helperFunctions = _helperFunctionsFactory.Create(project.Id, runId);

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(
                         helperFunctions))
            {
                engine.SetValue(functionName, delegateFunction);
            }

            var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
            engine.Execute(validScript);

            return helperFunctions.GetResults();
        }
    }
}