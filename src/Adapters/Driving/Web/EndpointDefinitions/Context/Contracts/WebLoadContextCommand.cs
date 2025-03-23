namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebLoadContextCommand(IEnumerable<string> LoaderIds);
