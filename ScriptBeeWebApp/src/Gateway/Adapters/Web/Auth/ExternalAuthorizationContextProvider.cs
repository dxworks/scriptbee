using System.Security.Claims;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Web.Auth;

public sealed class ExternalAuthorizationContextProvider(
    IResourceMemberService resourceMemberService
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

    private static (UserId userId, List<UserGroup> groups) ExtractFromClaims(
        HttpContext httpContext
    )
    {
        // TODO FIXIT(#328): normalize userid and groups from claims and obtain user id from sub
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var groups = httpContext.User.FindAll("groups").Select(c => c.Value).ToList();

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
