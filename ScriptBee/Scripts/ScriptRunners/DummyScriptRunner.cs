using ScriptBee.Models.Dummy;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public abstract class DummyScriptRunner : IScriptRunner
    {
        protected readonly ValidScriptExtractor ScriptExtractor;

        protected DummyScriptRunner()
        {
        }

        protected DummyScriptRunner(ValidScriptExtractor scriptExtractor)
        {
            ScriptExtractor = scriptExtractor;
        }

        public abstract void RunScript(DummyModel dummyModel, string script);
    }
}