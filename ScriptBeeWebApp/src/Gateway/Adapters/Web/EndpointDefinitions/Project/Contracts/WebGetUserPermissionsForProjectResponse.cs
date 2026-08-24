namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetUserPermissionsForProjectResponse(
    string? Role,
    IEnumerable<string> Permissions
);
