using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ScriptBee.Adapters.Auth.Contracts;

namespace ScriptBee.Adapters.Auth;

public sealed class ExternalAuthorizationActionAuthorizationHandler(
    IAuthorizeExternally authorizeExternally,
    IExternalAuthorizationContextProvider externalAuthorizationContextProvider
) : AuthorizationHandler<PermissionActionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionActionRequirement requirement
    )
    {
        ExternalAuthorizationRequest? request = null;
        var cancellationToken = CancellationToken.None;

        switch (context.Resource)
        {
            case HttpContext httpContext:
                cancellationToken = httpContext.RequestAborted;
                request = await externalAuthorizationContextProvider.BuildRequestAsync(
                    httpContext,
                    requirement.Action,
                    cancellationToken
                );
                break;
            case HubInvocationContext hubInvocationContext:
                cancellationToken = hubInvocationContext.Context.ConnectionAborted;
                request = await externalAuthorizationContextProvider.BuildRequestAsync(
                    hubInvocationContext,
                    requirement.Action,
                    cancellationToken
                );
                break;
        }

        if (request is null)
        {
            return;
        }

        var isAllowed = await authorizeExternally.IsAllowed(request, cancellationToken);
        if (isAllowed)
        {
            context.Succeed(requirement);
        }
    }
}
