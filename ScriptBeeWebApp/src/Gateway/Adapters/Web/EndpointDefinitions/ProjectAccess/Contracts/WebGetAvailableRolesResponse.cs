namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebGetAvailableRolesResponse(IEnumerable<WebRoleInfo> Roles);
