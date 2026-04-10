namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebUpdateScriptCommand(string? Name, IEnumerable<WebScriptParameter>? Parameters);
