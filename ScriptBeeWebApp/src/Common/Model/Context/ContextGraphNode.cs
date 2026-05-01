namespace ScriptBee.Domain.Model.Context;

public record ContextGraphNode(
    string Id,
    string Label,
    string Type,
    string? Loader,
    Dictionary<string, object> Properties
);
