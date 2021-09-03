using System.Collections.Generic;

namespace ScriptBeePlugin
{
    public interface IModelLoader
    {
        public Dictionary<string, Dictionary<string, ScriptBeeModel>> LoadModel(List<string> fileContents, Dictionary<string, object> configuration = null);

        public string GetName();
    }
}