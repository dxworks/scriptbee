using DummyPlugin;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public abstract class DummyScriptRunner : IScriptRunner
    {
        protected readonly ValidScriptExtractor scriptExtractor;

        protected DummyScriptRunner()
        {
        }

        protected DummyScriptRunner(ValidScriptExtractor scriptExtractor)
        {
            this.scriptExtractor = scriptExtractor;
        }

        public abstract void RunScript(DummyModel dummyModel, string script);
    }
}