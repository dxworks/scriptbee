using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebContextGraphNode(
    string Id,
    string Label,
    string Type,
    string? Loader,
    Dictionary<string, object> Properties
)
{
    public static WebContextGraphNode Map(ContextGraphNode node) =>
        new(node.Id, node.Label, node.Type, node.Loader, node.Properties);
}
