using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Rest.Contracts;

public record RestContextGraphNode(
    string Id,
    string Label,
    string Type,
    string? Loader,
    Dictionary<string, object> Properties
)
{
    public ContextGraphNode Map() => new(Id, Label, Type, Loader, Properties);
}
