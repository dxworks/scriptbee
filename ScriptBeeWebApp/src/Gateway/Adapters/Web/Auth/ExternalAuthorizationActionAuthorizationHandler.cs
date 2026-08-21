using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Web.Auth;

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
        if (context.Resource is not HttpContext httpContext)
        {
            return;
        }

        var cancellationToken = httpContext.RequestAborted;

        var request = await externalAuthorizationContextProvider.BuildRequestAsync(
            httpContext,
            requirement.Action,
            cancellationToken
        );

        var isAllowed = await authorizeExternally.IsAllowed(request, cancellationToken);

        if (isAllowed)
        {
            context.Succeed(requirement);
        }
    }
}
