namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebLoadContextCommand(IDictionary<string, List<string>> FilesToLoad);
