using System;
using System.Collections.Generic;
using IronPython.Hosting;
using ScriptBee.Models;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts
{
    public class PythonDummyScriptRunner : IDummyScriptRunner
    {
        private readonly IValidScriptExtractor _scriptExtractor;

        public PythonDummyScriptRunner(IValidScriptExtractor scriptExtractor)
        {
            _scriptExtractor = scriptExtractor;
        }
        
        public void RunScript(DummyModel dummyModel, string script)
        {
            var pythonEngine = Python.CreateEngine();
            var scriptScope = pythonEngine.CreateScope(new Dictionary<string, object>
            {
                {
                    "model", dummyModel
                },
                {
                    "print", new Action<object>(obj => Console.WriteLine(obj))
                }
            });

            var validScript = _scriptExtractor.ExtractValidScript(script);
            pythonEngine.Execute(validScript, scriptScope);
        }
    }
}