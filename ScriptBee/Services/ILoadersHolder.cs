using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Services;

// todo refactor loaders holder
public interface ILoadersHolder
{
    public void AddLoaderToDictionary(IModelLoader loader);

    public IModelLoader? GetModelLoader(string modelName);

    public IEnumerable<IModelLoader> GetAllLoaders();
}
