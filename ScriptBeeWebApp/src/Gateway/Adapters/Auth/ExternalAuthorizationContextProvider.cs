using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OneOf;
using ScriptBee.Adapters.Auth.Config;
using ScriptBee.Adapters.Auth.Contracts;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Adapters.Auth;

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
        var claimsPrincipal = httpContext.User;

        string? requestedProjectId = null;
        if (
            routeData.Values.TryGetValue("projectId", out var projectIdObj)
            && projectIdObj is string projectId
        )
        {
            requestedProjectId = projectId;
        }

        if (IsProjectToken(claimsPrincipal))
        {
            return BuildProjectTokenRequest(claimsPrincipal, action, requestedProjectId);
        }

        var authConfig = authConfigOptions.Value;
        var userId = (
            await CurrentUser.ExtractUserIdFromClaims(
                claimsPrincipal,
                authConfig,
                manageUsersUseCase,
                cancellationToken
            )
        )!.Value;
        var groups = CurrentUser.ExtractGroupsFromClaims(claimsPrincipal, authConfig);

        if (requestedProjectId is not null)
        {
            return await GetProjectRequest(
                action,
                userId,
                groups,
                ProjectId.FromValue(requestedProjectId),
                cancellationToken
            );
        }

        return GetGlobalRequest(action, userId, groups);
    }

    public async Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HubInvocationContext hubInvocationContext,
        string action,
        CancellationToken cancellationToken
    )
    {
        var claimsPrincipal = hubInvocationContext.Context.User;
        if (claimsPrincipal is null)
        {
            return GetGlobalRequest(action, new UserId(""), []);
        }

        var requestedProjectId = ExtractProjectIdFromHubInvocation(hubInvocationContext);

        if (IsProjectToken(claimsPrincipal))
        {
            return BuildProjectTokenRequest(claimsPrincipal, action, requestedProjectId);
        }

        var authConfig = authConfigOptions.Value;
        var userId = (
            await CurrentUser.ExtractUserIdFromClaims(
                claimsPrincipal,
                authConfig,
                manageUsersUseCase,
                cancellationToken
            )
        )!.Value;
        var groups = CurrentUser.ExtractGroupsFromClaims(claimsPrincipal, authConfig);

        if (requestedProjectId is not null)
        {
            return await GetProjectRequest(
                action,
                userId,
                groups,
                ProjectId.FromValue(requestedProjectId),
                cancellationToken
            );
        }

        return GetGlobalRequest(action, userId, groups);
    }

    private static bool IsProjectToken(ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.HasClaim("token_type", "project_token");

    private static ExternalAuthorizationRequest BuildProjectTokenRequest(
        ClaimsPrincipal claimsPrincipal,
        string action,
        string? requestedProjectId
    )
    {
        var tokenId = claimsPrincipal.FindFirst("token_id")?.Value;
        var tokenProjectId = claimsPrincipal.FindFirst("project_id")?.Value;
        var tokenRole = claimsPrincipal.FindFirst("role")?.Value;

        var isMatchingProject = requestedProjectId is not null
            && string.Equals(tokenProjectId, requestedProjectId, StringComparison.Ordinal);

        return new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = $"project-token:{tokenId}",
                    Groups = [],
                },
                Action = action,
                Resource = new ExternalAuthorizationResource
                {
                    Type = requestedProjectId is not null ? "project" : "global",
                    Id = requestedProjectId,
                    Role = isMatchingProject ? tokenRole : null,
                },
            },
        };
    }

    private static string? ExtractProjectIdFromHubInvocation(
        HubInvocationContext hubInvocationContext
    )
    {
        var parameters = hubInvocationContext.HubMethod.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            if (
                string.Equals(parameters[i].Name, "projectId", StringComparison.OrdinalIgnoreCase)
                && i < hubInvocationContext.HubMethodArguments.Count
                && hubInvocationContext.HubMethodArguments[i] is string projectId
            )
            {
                return projectId;
            }
        }

        return null;
    }

    private async Task<ExternalAuthorizationRequest> GetProjectRequest(
        string action,
        UserId userId,
        List<UserGroup> groups,
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
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
