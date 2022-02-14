using System.Collections.Generic;
using System.IO;
using HelperFunctions;
using IronPython.Hosting;
using ScriptBee.Config;
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

            var outputFolderPath = Path.Combine(ConfigFolders.PathToResults, project.Id);

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(
                outputFolderPath))
            {
                dictionary.Add(functionName, delegateFunction);
            }

            var scriptScope = pythonEngine.CreateScope(dictionary);

            var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
            pythonEngine.Execute(validScript, scriptScope);
        }
    }
}