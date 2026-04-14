namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebProjectContextGenerateClassesRequest(
    List<string>? Languages,
    string? TransferFormat = null
);
