namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebGetAvailableScriptTypesResponse(IEnumerable<WebScriptLanguage> Data);
