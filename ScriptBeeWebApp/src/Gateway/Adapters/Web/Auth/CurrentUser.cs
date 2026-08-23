using System.Security.Claims;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.User;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Auth;

public sealed class CurrentUser(UserId id)
{
    public UserId Id => id;

    public static ValueTask<CurrentUser?> BindAsync(HttpContext context)
    {
        var user = context.User;

        var authConfigOptions = context.RequestServices.GetRequiredService<
            IOptions<AuthenticationConfig>
        >();
        var authConfig = authConfigOptions.Value;

        if (authConfig.IsDevelopment)
        {
            return ValueTask.FromResult<CurrentUser?>(new CurrentUser(new UserId("")));
        }

        if (user.Identity?.IsAuthenticated != true)
        {
            return ValueTask.FromResult<CurrentUser?>(null);
        }

        var userId = ExtractUserIdFromClaims(user, authConfig);
        return ValueTask.FromResult<CurrentUser?>(new CurrentUser(userId));
    }

    public static UserId ExtractUserIdFromClaims(
        ClaimsPrincipal claimsPrincipal,
        AuthenticationConfig authConfig
    )
    {
        var userId =
            authConfig.UserIdClaim != null
                ? claimsPrincipal.FindFirst(authConfig.UserIdClaim)?.Value
                : claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        userId ??= "";

        return new UserId(userId);
    }

    public static List<UserGroup> ExtractGroupsFromClaims(
        ClaimsPrincipal claimsPrincipal,
        AuthenticationConfig authConfig
    )
    {
        var groups =
            authConfig.GroupsClaim == null
                ? []
                : claimsPrincipal.FindAll(authConfig.GroupsClaim).Select(c => c.Value).ToList();

        return [.. groups.Select(g => new UserGroup(g))];
    }
}
