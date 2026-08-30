using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess;

public class ManageProjectAccessEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IManageUsersUseCase, ManageUsersService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/members", GetProjectMembers)
            .WithTags("Projects", "Access")
            .WithSummary("Get project members")
            .WithDescription("Returns all users and groups with their roles for the project.")
            .RequireAction("project:manage_access");

        app.MapPut("/api/projects/{projectId}/members/{memberId}", UpdateProjectMember)
            .WithTags("Projects", "Access")
            .WithSummary("Update project member role")
            .WithDescription("Assigns or updates the role for a user or group on the project.")
            .WithRequestValidation<WebUpdateProjectMemberCommand>()
            .RequireAction("project:manage_access");

        app.MapDelete("/api/projects/{projectId}/members/{memberId}", RemoveProjectMember)
            .WithTags("Projects", "Access")
            .WithSummary("Remove project member")
            .WithDescription("Removes a user or group's access from the project.")
            .RequireAction("project:manage_access");

        app.MapGet("/api/users", GetAllUsers)
            .WithTags("Users")
            .WithSummary("Get all users")
            .WithDescription("Returns all registered users for search-ahead.")
            .RequireAuthorization();

        app.MapGet("/api/roles", GetAvailableRoles)
            .WithTags("Roles")
            .WithSummary("Get available roles")
            .WithDescription(
                "Returns all roles available for assignment, as defined in the authorization system."
            )
            .RequireAuthorization();
    }

    private static async Task<Ok<WebGetProjectMembersResponse>> GetProjectMembers(
        [FromRoute] string projectId,
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var members = await useCase.GetProjectMembers(
            ProjectId.FromValue(projectId),
            cancellationToken
        );

        return TypedResults.Ok(
            new WebGetProjectMembersResponse(members.Select(WebProjectMember.Map))
        );
    }

    private static async Task<NoContent> UpdateProjectMember(
        [FromRoute] string projectId,
        [FromRoute] string memberId,
        [FromBody] WebUpdateProjectMemberCommand command,
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.UpdateProjectMember(
            new UpdateProjectMemberCommand(
                ProjectId.FromValue(projectId),
                memberId,
                command.MemberType,
                new UserRole(command.Role)
            ),
            cancellationToken
        );

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> RemoveProjectMember(
        [FromRoute] string projectId,
        [FromRoute] string memberId,
        [FromQuery] string memberType,
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.RemoveProjectMember(
            new RemoveProjectMemberCommand(ProjectId.FromValue(projectId), memberId, memberType),
            cancellationToken
        );

        return TypedResults.NoContent();
    }

    private static async Task<Ok<WebGetAllUsersResponse>> GetAllUsers(
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var users = await useCase.GetAllUsers(cancellationToken);

        return TypedResults.Ok(
            new WebGetAllUsersResponse(users.Select(u => new WebUserInfo(u.Id.Value, u.Name)))
        );
    }

    private static async Task<Ok<WebGetAvailableRolesResponse>> GetAvailableRoles(
        IManageUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var roles = await useCase.GetAvailableRoles(cancellationToken);

        return TypedResults.Ok(
            new WebGetAvailableRolesResponse(
                roles.Select(r => new WebRoleInfo(r.Id, r.Description))
            )
        );
    }
}
