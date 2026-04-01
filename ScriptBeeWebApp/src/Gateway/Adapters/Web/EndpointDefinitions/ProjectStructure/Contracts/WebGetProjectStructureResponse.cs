namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebGetProjectStructureResponse(IEnumerable<WebProjectStructureNode> Data);
