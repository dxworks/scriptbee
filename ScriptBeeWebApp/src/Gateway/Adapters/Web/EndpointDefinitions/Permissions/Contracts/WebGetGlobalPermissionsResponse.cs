namespace ScriptBee.Web.EndpointDefinitions.Permissions.Contracts;

public record WebGetGlobalPermissionsResponse(IEnumerable<string> Permissions);
