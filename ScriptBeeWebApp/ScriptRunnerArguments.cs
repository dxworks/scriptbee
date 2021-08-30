using ScriptBee.Models.Dummy;

namespace ScriptBeeWebApp
{
    public class ScriptRunnerArguments
    {
        public string ModelType { get; set; }

        public string ModelJsonContent { get; set; }

        public string ScriptType { get; set; }

        public string ScriptContent { get; set; }
    }
}