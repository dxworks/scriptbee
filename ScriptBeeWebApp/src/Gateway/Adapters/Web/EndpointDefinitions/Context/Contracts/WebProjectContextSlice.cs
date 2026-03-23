using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebProjectContextSlice(string Model, IEnumerable<string> PluginIds)
{
    public static WebProjectContextSlice Map(ContextSlice contextSlice)
    {
        return new WebProjectContextSlice(contextSlice.Model, contextSlice.PluginIds);
    }
}
