namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebGetUserPermissionsForProjectResponse(
    string? Role,
    IEnumerable<string> Permissions
);
