using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.PluginManager
{
    public interface ILoadersHolder
    {
        public void AddLoaderToDictionary(IModelLoader loader);
        
        public IModelLoader? GetModelLoader(string modelName);

        public IEnumerable<IModelLoader> GetAllLoaders();
    }
}