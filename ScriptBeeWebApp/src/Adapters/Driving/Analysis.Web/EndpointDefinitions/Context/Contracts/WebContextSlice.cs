using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebContextSlice(string Model, IEnumerable<string> PluginIds)
{
    public static WebContextSlice Map(ContextSlice contextSlice)
    {
        return new WebContextSlice(contextSlice.Model, contextSlice.PluginIds);
    }
}
