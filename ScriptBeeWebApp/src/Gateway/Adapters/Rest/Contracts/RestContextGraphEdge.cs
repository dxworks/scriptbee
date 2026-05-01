using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Rest.Contracts;

public record RestContextGraphEdge(string Source, string Target, string Label)
{
    public ContextGraphEdge Map() => new(Source, Target, Label);
}
