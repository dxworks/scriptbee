using System.Collections.Generic;
using HelperFunctions;
using IronPython.Hosting;
using ScriptBee.ProjectContext;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class PythonScriptRunner : IScriptRunner
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;

        private readonly ValidScriptExtractor _scriptExtractor;

        public PythonScriptRunner(IHelperFunctionsMapper helperFunctionsMapper, ValidScriptExtractor scriptExtractor)
        {
            _helperFunctionsMapper = helperFunctionsMapper;
            _scriptExtractor = scriptExtractor;
        }

        public void Run(Project project, string scriptContent)
        {
            var pythonEngine = Python.CreateEngine();
            var dictionary = new Dictionary<string, object>
            {
                {
                    "project", project
                },
            };

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(
                project.ProjectId))
            {
                dictionary.Add(functionName, delegateFunction);
            }

            var scriptScope = pythonEngine.CreateScope(dictionary);

            var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
            pythonEngine.Execute(validScript, scriptScope);
        }
    }
}