using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebContextGraphEdge(string Source, string Target, string Label)
{
    public static WebContextGraphEdge Map(ContextGraphEdge edge) =>
        new(edge.Source, edge.Target, edge.Label);
}
