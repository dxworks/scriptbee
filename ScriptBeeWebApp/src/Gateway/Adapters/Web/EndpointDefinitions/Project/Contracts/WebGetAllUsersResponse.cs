namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetAllUsersResponse(IEnumerable<WebUserInfo> Users);
