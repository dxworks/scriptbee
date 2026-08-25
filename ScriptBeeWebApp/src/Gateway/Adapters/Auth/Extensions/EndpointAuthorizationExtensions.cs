using Microsoft.AspNetCore.Builder;

namespace ScriptBee.Adapters.Auth.Extensions;

public static class EndpointAuthorizationExtensions
{
    public static RouteHandlerBuilder RequireAction(this RouteHandlerBuilder builder, string action)
    {
        return builder.RequireAuthorization(new AuthorizeActionAttribute(action));
    }

    public static HubEndpointConventionBuilder RequireAction(
        this HubEndpointConventionBuilder builder,
        string action
    )
    {
        return builder.RequireAuthorization(new AuthorizeActionAttribute(action));
    }
}
