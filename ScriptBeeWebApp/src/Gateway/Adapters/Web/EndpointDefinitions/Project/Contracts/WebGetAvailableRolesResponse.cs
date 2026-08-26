namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetAvailableRolesResponse(IEnumerable<WebRoleInfo> Roles);
