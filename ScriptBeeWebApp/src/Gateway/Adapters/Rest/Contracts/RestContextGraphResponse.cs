namespace ScriptBee.Rest.Contracts;

public record RestContextGraphResponse(
    IEnumerable<RestContextGraphNode> Nodes,
    IEnumerable<RestContextGraphEdge> Edges
);
