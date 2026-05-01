namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebContextGraphResponse(
    IEnumerable<WebContextGraphNode> Nodes,
    IEnumerable<WebContextGraphEdge> Edges
);
