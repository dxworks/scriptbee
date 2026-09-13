namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebGenerateClassesRequest(List<string>? Languages, string? TransferFormat = null);
