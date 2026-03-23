using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Rest.Contracts;

public class RestContextSlice
{
    public required string Model { get; set; }

    public required IEnumerable<string> PluginIds { get; set; }

    public ContextSlice Map()
    {
        return new ContextSlice(Model, PluginIds);
    }
}
