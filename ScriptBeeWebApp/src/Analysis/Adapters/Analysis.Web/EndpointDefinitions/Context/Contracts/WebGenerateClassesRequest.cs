namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebGenerateClassesRequest(List<string>? Languages, string? TransferFormat = null);
