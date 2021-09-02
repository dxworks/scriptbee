using System.Collections.Generic;
using DummyPlugin;
using HelperFunctions;
using IronPython.Hosting;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class PythonDummyScriptRunner : DummyScriptRunner
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;

        public PythonDummyScriptRunner(IHelperFunctionsMapper helperFunctionsMapper) : base(
            new PythonValidScriptExtractor())
        {
            _helperFunctionsMapper = helperFunctionsMapper;
        }

        public override void RunScript(DummyModel dummyModel, string script)
        {
            var pythonEngine = Python.CreateEngine();
            var dictionary = new Dictionary<string, object>
            {
                {
                    "model", dummyModel
                },
            };

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary())
            {
                dictionary.Add(functionName, delegateFunction);
            }

            var scriptScope = pythonEngine.CreateScope(dictionary);

            var validScript = scriptExtractor.ExtractValidScript(script);
            pythonEngine.Execute(validScript, scriptScope);
        }
    }
}