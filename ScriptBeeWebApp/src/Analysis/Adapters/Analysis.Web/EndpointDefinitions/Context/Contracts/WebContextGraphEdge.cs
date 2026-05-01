using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebContextGraphEdge(string Source, string Target, string Label)
{
    public static WebContextGraphEdge Map(ContextGraphEdge edge) =>
        new(edge.Source, edge.Target, edge.Label);
}
