namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebGetProjectMembersResponse(IEnumerable<WebProjectMember> Members);
