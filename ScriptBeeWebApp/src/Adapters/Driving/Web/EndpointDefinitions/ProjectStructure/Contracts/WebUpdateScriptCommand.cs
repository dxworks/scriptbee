namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebUpdateScriptCommand(string? Language, IEnumerable<WebScriptParameter>? Parameters);
