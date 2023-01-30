using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default;

public class ContextHelperFunctions : IHelperFunctions
{
    public object ContextGetValue(IContext context, string entityName, string loaderName)
    {
        return context.Models[new Tuple<string, string>(entityName, loaderName)];
    }
}
