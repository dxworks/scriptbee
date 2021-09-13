using System.Collections.Generic;
using System.IO;
using ScriptBeePlugin;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class TestModelLoader : IModelLoader
    {
        public Dictionary<string, Dictionary<string, ScriptBeeModel>> LoadModel(List<Stream> fileStreams,
            Dictionary<string, object> configuration = null)
        {
            return new Dictionary<string, Dictionary<string, ScriptBeeModel>>();
        }

        public string GetName()
        {
            return "";
        }
    }
}