using ScriptBee.Domain.Model.Context;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebContextGraphEdge(string Source, string Target, string Label)
{
    public static WebContextGraphEdge Map(ContextGraphEdge edge) =>
        new(edge.Source, edge.Target, edge.Label);
}
