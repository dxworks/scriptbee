using Microsoft.Extensions.Options;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.Auth.Contracts;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Auth;

public sealed class ExternalAuthorizationContextProvider(
    IGetResourceRole getResourceRole,
    IOptions<AuthenticationConfig> authConfigOptions,
    IManageUsersUseCase manageUsersUseCase
) : IExternalAuthorizationContextProvider
{
    public async Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HttpContext httpContext,
        string action,
        CancellationToken cancellationToken
    )
    {
        var routeData = httpContext.GetRouteData();
        var authConfig = authConfigOptions.Value;
        var claimsPrincipal = httpContext.User;
        var userId = (
            await CurrentUser.ExtractUserIdFromClaims(
                claimsPrincipal,
                authConfig,
                manageUsersUseCase,
                httpContext.RequestAborted
            )
        )!.Value;
        var groups = CurrentUser.ExtractGroupsFromClaims(claimsPrincipal, authConfig);

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

    private async Task<ExternalAuthorizationRequest> GetProjectRequest(
        string action,
        UserId userId,
        List<UserGroup> groups,
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        // TODO: should cache this mongo query
        var resourceRole = await getResourceRole.GetRole(
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
