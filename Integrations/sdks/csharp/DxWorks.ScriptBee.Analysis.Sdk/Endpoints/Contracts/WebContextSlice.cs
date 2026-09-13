using ScriptBee.Domain.Model.Context;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebContextSlice(string Model, IEnumerable<string> PluginIds)
{
    public static WebContextSlice Map(ContextSlice contextSlice)
    {
        return new WebContextSlice(contextSlice.Model, contextSlice.PluginIds);
    }
}
