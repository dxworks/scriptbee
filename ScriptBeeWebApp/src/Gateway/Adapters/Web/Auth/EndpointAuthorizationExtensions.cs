namespace ScriptBee.Web.Auth;

public static class EndpointAuthorizationExtensions
{
    public static RouteHandlerBuilder RequireAction(this RouteHandlerBuilder builder, string action)
    {
        return builder.RequireAuthorization(new AuthorizeActionAttribute(action));
    }
}
