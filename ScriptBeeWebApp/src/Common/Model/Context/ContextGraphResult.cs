namespace ScriptBee.Domain.Model.Context;

public record ContextGraphResult(
    IEnumerable<ContextGraphNode> Nodes,
    IEnumerable<ContextGraphEdge> Edges
);
