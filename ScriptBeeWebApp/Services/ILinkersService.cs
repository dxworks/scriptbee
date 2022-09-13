using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBeeWebApp.Services;

public interface ILinkersService
{
    IEnumerable<string> GetSupportedLinkers();

    IModelLinker? GetLinker(string name);
}
