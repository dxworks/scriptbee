using System.Security.Claims;
using Microsoft.Extensions.Options;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Web.Auth.Contracts;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Auth;

public sealed class ExternalAuthorizationContextProvider(
    IResourceMemberService resourceMemberService,
    IOptions<AuthenticationConfig> authConfigOptions
) : IExternalAuthorizationContextProvider
{
    public async Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HttpContext httpContext,
        string action,
        CancellationToken cancellationToken
    )
    {
        var routeData = httpContext.GetRouteData();
        var (userId, groups) = ExtractFromClaims(httpContext);

        if (
            routeData.Values.TryGetValue("projectId", out var projectIdObj)
            && projectIdObj is string projectId
        )
        {
            return await GetProjectRequest(
                action,
                userId,
                groups,
                ProjectId.FromValue(projectId),
                cancellationToken
            );
        }

        return GetGlobalRequest(action, userId, groups);
    }

    private (UserId userId, List<UserGroup> groups) ExtractFromClaims(HttpContext httpContext)
    {
        var authConfig = authConfigOptions.Value;
        var claimsPrincipal = httpContext.User;

        var userId =
            authConfig.UserIdClaim != null
                ? claimsPrincipal.FindFirst(authConfig.UserIdClaim)?.Value
                : claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        userId ??= "";

        var groups =
            authConfig.GroupsClaim == null
                ? []
                : claimsPrincipal.FindAll(authConfig.GroupsClaim).Select(c => c.Value).ToList();

        return new ValueTuple<UserId, List<UserGroup>>(
            new UserId(userId),
            [.. groups.Select(g => new UserGroup(g))]
        );
    }

    private async Task<ExternalAuthorizationRequest> GetProjectRequest(
        string action,
        UserId userId,
        List<UserGroup> groups,
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var resourceRole = await resourceMemberService.GetResourceRole(
            userId,
            groups,
            OneOf<ProjectId>.FromT0(projectId),
            cancellationToken
        );

        return new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = userId.Value,
                    Groups = [.. groups.Select(g => g.Value)],
                },
                Action = action,
                Resource = new ExternalAuthorizationResource
                {
                    Type = "project",
                    Id = projectId.Value,
                    Role = resourceRole?.Value,
                },
            },
        };
    }

    private static ExternalAuthorizationRequest GetGlobalRequest(
        string action,
        UserId userId,
        List<UserGroup> groups
    )
    {
        return new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = userId.Value,
                    Groups = [.. groups.Select(g => g.Value)],
                },
                Action = action,
                Resource = new ExternalAuthorizationResource { Type = "global" },
            },
        };
    }
}
