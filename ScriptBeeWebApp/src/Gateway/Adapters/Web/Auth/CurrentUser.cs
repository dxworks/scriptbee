using System.Security.Claims;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.User;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Auth;

public sealed class CurrentUser(UserId id)
{
    public UserId Id => id;

    public static async ValueTask<CurrentUser?> BindAsync(HttpContext context)
    {
        var user = context.User;

        var authConfigOptions = context.RequestServices.GetRequiredService<
            IOptions<AuthenticationConfig>
        >();
        var authConfig = authConfigOptions.Value;
        var useCase = context.RequestServices.GetRequiredService<IManageUsersUseCase>();

        if (authConfig.IsDevelopment)
        {
            return new CurrentUser(new UserId(""));
        }

        if (user.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = await ExtractUserIdFromClaims(
            user,
            authConfig,
            useCase,
            context.RequestAborted
        );

        return !userId.HasValue ? null : new CurrentUser(userId.Value);
    }

    public static async Task<UserId?> ExtractUserIdFromClaims(
        ClaimsPrincipal claimsPrincipal,
        AuthenticationConfig authConfig,
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var userId =
            authConfig.UserIdClaim != null
                ? claimsPrincipal.FindFirst(authConfig.UserIdClaim)?.Value
                : claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName =
            claimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value
            ?? claimsPrincipal.FindFirst(ClaimTypes.Surname)?.Value
            ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value
            ?? claimsPrincipal.FindFirst(ClaimTypes.WindowsAccountName)?.Value
            ?? "";

        if (userId == null)
        {
            return null;
        }

        return await useCase.GetUserId(userId, userName, cancellationToken);
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
