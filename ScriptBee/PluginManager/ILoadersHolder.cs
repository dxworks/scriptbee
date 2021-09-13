using System.Collections.Generic;
using ScriptBeePlugin;

namespace ScriptBee.PluginManager
{
    public interface ILoadersHolder
    {
        public void AddLoaderToDictionary(IModelLoader loader);
        
        public IModelLoader GetModelLoader(string modelName);

        public List<IModelLoader> GetAllLoaders();
    }
}