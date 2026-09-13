namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebContextGraphResponse(
    IEnumerable<WebContextGraphNode> Nodes,
    IEnumerable<WebContextGraphEdge> Edges
);
