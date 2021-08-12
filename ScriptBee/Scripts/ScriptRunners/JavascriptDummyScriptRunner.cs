using System;
using Jint;
using ScriptBee.Models.Dummy;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class JavascriptDummyScriptRunner : DummyScriptRunner
    {
        public JavascriptDummyScriptRunner() : base(new JavascriptValidScriptExtractor())
        {
        }

        public override void RunScript(DummyModel dummyModel, string script)
        {
            var engine = new Engine();
            engine.SetValue("model", dummyModel);
            engine.SetValue("print", new Action<object>(obj => Console.WriteLine(obj)));


            var validScript = ScriptExtractor.ExtractValidScript(script);
            engine.Execute(validScript);
        }
    }
}