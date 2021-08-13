using System;
using Jint;
using ScriptBee.Models.Dummy;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class CSharpDummyScriptRunner : DummyScriptRunner
    {
        public CSharpDummyScriptRunner() : base(new CSharpValidScriptExtractor())
        {
        }

        public override void RunScript(DummyModel dummyModel, string script)
        {
            
        }
    }
}