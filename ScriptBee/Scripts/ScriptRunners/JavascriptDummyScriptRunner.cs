using Jint;
using ScriptBee.Models.Dummy;
using ScriptBee.Utils;
using ScriptBee.Utils.ValidScriptExtractors;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class JavascriptDummyScriptRunner : DummyScriptRunner
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;

        public JavascriptDummyScriptRunner(IHelperFunctionsMapper helperFunctionsMapper) : base(
            new JavascriptValidScriptExtractor())
        {
            _helperFunctionsMapper = helperFunctionsMapper;
        }

        public override void RunScript(DummyModel dummyModel, string script)
        {
            var engine = new Engine();
            engine.SetValue("model", dummyModel);

            foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary())
            {
                engine.SetValue(functionName, delegateFunction);
            }

            var validScript = ScriptExtractor.ExtractValidScript(script);
            engine.Execute(validScript);
        }
    }
}