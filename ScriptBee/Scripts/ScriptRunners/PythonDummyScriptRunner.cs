using System;
using System.Collections.Generic;
using IronPython.Hosting;
using ScriptBee.Models.Dummy;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class PythonDummyScriptRunner : DummyScriptRunner
    {
        public PythonDummyScriptRunner() : base(new PythonValidScriptExtractor())
        {
        }

        public override void RunScript(DummyModel dummyModel, string script)
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

            var validScript = ScriptExtractor.ExtractValidScript(script);
            pythonEngine.Execute(validScript, scriptScope);
        }
    }
}