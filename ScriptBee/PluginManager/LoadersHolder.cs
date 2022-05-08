using System;
using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Scripting.Utils;

namespace ScriptBee.PluginManager
{
    public class LoadersHolder : ILoadersHolder
    {
        private Dictionary<string, IModelLoader> loaders = new Dictionary<string, IModelLoader>();

        public void AddLoaderToDictionary(IModelLoader loader)
        {
            string loaderName = loader.GetName();
            
            if (!loaders.ContainsKey(loaderName))
            {
                loaders.Add(loaderName,loader);
            }
            else
            {
                Console.Error.WriteLine($"ModelLoader with key {loaderName} already exists");
            }
        }

        public IModelLoader? GetModelLoader(string modelName)
        {
            if (loaders.TryGetValue(modelName, out var loader))
            {
                return loader;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<IModelLoader> GetAllLoaders()
        {
            return loaders.Select(pair => pair.Value);
        }
    }
}