using ScriptBee.Models.Dummy;

namespace ScriptBee.Scripts
{
    public interface IDummyScriptRunner : IScriptRunner
    {
        public void RunScript(DummyModel dummyModel, string script);
    }
}