namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebLoadContextCommand(IDictionary<string, List<string>> FilesToLoad);
