using System.IO;
using HelperFunctions;
using Jint;
using ScriptBee.Config;
using ScriptBee.ProjectContext;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class JavascriptScriptRunner : IScriptRunner
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;

        private readonly ValidScriptExtractor _scriptExtractor;


        public JavascriptScriptRunner(IHelperFunctionsMapper helperFunctionsMapper,
            ValidScriptExtractor scriptExtractor)
        {
            _helperFunctionsMapper = helperFunctionsMapper;
            _scriptExtractor = scriptExtractor;
        }

        public void Run(Project project, string scriptContent)
        {
            var engine = new Engine();
            engine.SetValue("project", project);

            var outputFolderPath = Path.Combine(ConfigFolders.PathToResults, project.ProjectId);

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(
                outputFolderPath))
            {
                engine.SetValue(functionName, delegateFunction);
            }

            var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
            engine.Execute(validScript);
        }
    }
}