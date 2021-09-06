using HelperFunctions;
using Jint;
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
            engine.SetValue("context", project.Context);

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(project.ProjectId))
            {
                engine.SetValue(functionName, delegateFunction);
            }

            var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
            engine.Execute(validScript);
        }
    }
}