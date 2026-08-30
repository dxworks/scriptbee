namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebGetAllUsersResponse(IEnumerable<WebUserInfo> Users);
