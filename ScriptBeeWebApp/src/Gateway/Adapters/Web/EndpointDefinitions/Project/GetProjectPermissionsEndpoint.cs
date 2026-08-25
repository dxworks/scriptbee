using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Project;

public class GetProjectPermissionsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IProjectPermissionsUseCase, ProjectPermissionsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/permissions", GetUserPermissionsForProject)
            .WithTags("Projects", "Permissions")
            .WithSummary("Get project permissions")
            .WithDescription("Get all the permissions for the user associated for the project.")
            .RequireAuthorization();
    }

    private static async Task<
        Ok<WebGetUserPermissionsForProjectResponse>
    > GetUserPermissionsForProject(
        CurrentUser currentUser,
        [FromRoute] string projectId,
        IProjectPermissionsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var query = new GetProjectPermissionsQuery(
            ProjectId.FromValue(projectId),
            currentUser.Id,
            currentUser.Groups
        );
        var userPermissions = await useCase.GetProjectPermissions(query, cancellationToken);

        if (userPermissions == null)
        {
            return TypedResults.Ok(new WebGetUserPermissionsForProjectResponse(null, []));
        }

        return TypedResults.Ok(
            new WebGetUserPermissionsForProjectResponse(
                userPermissions.Role.Value,
                userPermissions.Permissions
            )
        );
    }
}
