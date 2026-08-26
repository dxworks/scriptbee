namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetProjectMembersResponse(IEnumerable<WebProjectMember> Members);
