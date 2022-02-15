using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ScriptBeePlugin
{
    public interface IModelLoader
    {
        public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
            Dictionary<string, object> configuration = null);

        public string GetName();
    }
}