using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Project;

public class ManageProjectAccessEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetProjectMembersUseCase, GetProjectMembersService>();
        services.AddSingleton<IUpdateProjectMemberUseCase, UpdateProjectMemberService>();
        services.AddSingleton<IRemoveProjectMemberUseCase, RemoveProjectMemberService>();
        services.AddSingleton<IGetAllUsersUseCase, GetAllUsersService>();
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
    }

    private static async Task<Ok<WebGetProjectMembersResponse>> GetProjectMembers(
        [FromRoute] string projectId,
        IGetProjectMembersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var members = await useCase.GetProjectMembers(
            ProjectId.FromValue(projectId),
            cancellationToken
        );

        return TypedResults.Ok(
            new WebGetProjectMembersResponse(
                members.Select(m => new WebProjectMember(m.MemberId, m.MemberType, m.Role.Value))
            )
        );
    }

    private static async Task<NoContent> UpdateProjectMember(
        [FromRoute] string projectId,
        [FromRoute] string memberId,
        [FromBody] WebUpdateProjectMemberCommand command,
        IUpdateProjectMemberUseCase useCase,
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
        IRemoveProjectMemberUseCase useCase,
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
        IGetAllUsersUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var users = await useCase.GetAllUsers(cancellationToken);

        return TypedResults.Ok(
            new WebGetAllUsersResponse(users.Select(u => new WebUserInfo(u.Id.Value, u.Name)))
        );
    }
}
