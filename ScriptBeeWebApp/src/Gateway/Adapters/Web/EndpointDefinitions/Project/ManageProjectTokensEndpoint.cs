// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
// using ScriptBee.Adapters.Auth.Extensions;
// using ScriptBee.Common.Web;
// using ScriptBee.Domain.Model.Project;
// using ScriptBee.UseCases.Gateway;
// using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
//
// namespace ScriptBee.Web.EndpointDefinitions.Project;
//
// public class ManageProjectTokensEndpoint : IEndpointDefinition
// {
//     public void DefineServices(IServiceCollection services)
//     {
//     }
//
//     public void DefineEndpoints(IEndpointRouteBuilder app)
//     {
//         app.MapGet("/api/projects/{projectId}/tokens", GetProjectTokens)
//             .WithTags("Projects", "Access")
//             .WithSummary("Get project tokens")
//             .WithDescription("Returns all the tokens associated with the project tokens.")
//             .RequireAction("project:manage_access");
//
//         app.MapPost("/api/projects/{projectId}/tokens", CreateProjectToken)
//             .WithTags("Projects", "Access")
//             .WithSummary("Create project token")
//             .WithDescription("Creates a new project token")
//             .RequireAction("token:create");
//
//         app.MapDelete("/api/projects/{projectId}/tokens/{tokenId}", RemoveProjectToken)
//             .WithTags("Projects", "Access")
//             .WithSummary("Remove project token")
//             .WithDescription("Removes a token from the project.")
//             .RequireAction("token:delete");
//     }
//
//     private static async Task<Ok<WebGetProjectMembersResponse>> GetProjectTokens(
//         [FromRoute] string projectId,
//         IGetProjectMembersUseCase useCase,
//         CancellationToken cancellationToken
//     )
//     {
//         var members = await useCase.GetProjectMembers(
//             ProjectId.FromValue(projectId),
//             cancellationToken
//         );
//
//         return TypedResults.Ok(
//             new WebGetProjectMembersResponse(
//                 members.Select(m => new WebProjectMember(m.MemberId, m.MemberType, m.Role.Value))
//             )
//         );
//     }
//
//
//     private static async Task<Ok<WebGetAvailableRolesResponse>> CreateProjectToken(
//         [FromRoute] string projectId,
//         IGetAvailableRolesUseCase useCase,
//         CancellationToken cancellationToken
//     )
//     {
//         var roles = await useCase.GetAvailableRoles(cancellationToken);
//
//         return TypedResults.Ok(
//             new WebGetAvailableRolesResponse(
//                 roles.Select(r => new WebRoleInfo(r.Id, r.Description))
//             )
//         );
//     }
//
//     private static async Task<NoContent> RemoveProjectToken(
//         [FromRoute] string projectId,
//         [FromRoute] string tokenId,
//         IRemoveProjectMemberUseCase useCase,
//         CancellationToken cancellationToken
//     )
//     {
//         await useCase.RemoveProjectMember(
//             new RemoveProjectMemberCommand(ProjectId.FromValue(projectId), ),
//             cancellationToken
//         );
//
//         return TypedResults.NoContent();
//     }
// }
