namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebUpdateScriptCommand(IEnumerable<WebScriptParameter>? Parameters);
