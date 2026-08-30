namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebGetProjectTokensResponse(IEnumerable<WebProjectToken> Tokens);
