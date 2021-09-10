using System.Collections.Generic;
using System.IO;

namespace ScriptBeePlugin
{
    public interface IModelLoader
    {
        public Dictionary<string, Dictionary<string, ScriptBeeModel>> LoadModel(List<Stream> fileStreams, Dictionary<string, object> configuration = null);

        public string GetName();
    }
}