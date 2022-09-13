using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBeeWebApp.Services;

public interface ILoadersService
{
    IEnumerable<string> GetSupportedLoaders();

    IModelLoader? GetLoader(string name);

    ISet<string> GetAcceptedModules();
}
