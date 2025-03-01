namespace ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

public record WebGetProjectInstanceInfo(
    string Id,
    IEnumerable<string> Loaders,
    IEnumerable<string> Linkers,
    IDictionary<string, IEnumerable<string>> LoadedModels,
    DateTimeOffset CreationDate
);
