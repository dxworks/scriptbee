namespace ScriptBee.Domain.Model.Context;

public record ContextSlice(string Model, IEnumerable<string> PluginIds);
