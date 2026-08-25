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
        var authConfig = authConfigOptions.Value;
        var claimsPrincipal = httpContext.User;
        var userId = (
            await CurrentUser.ExtractUserIdFromClaims(
                claimsPrincipal,
                authConfig,
                manageUsersUseCase,
                cancellationToken
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

        var projectId = ExtractProjectIdFromHubInvocation(hubInvocationContext);
        if (projectId is not null)
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
