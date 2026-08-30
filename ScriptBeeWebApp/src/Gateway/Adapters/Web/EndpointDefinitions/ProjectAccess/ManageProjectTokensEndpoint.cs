using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess;

public class ManageProjectTokensEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IManageProjectTokensUseCase, ManageProjectTokensService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/tokens", GetProjectTokens)
            .WithTags("Projects", "Access")
            .WithSummary("Get project tokens")
            .WithDescription("Returns all the tokens associated with the project tokens.")
            .RequireAction("project:manage_access");

        app.MapPost("/api/projects/{projectId}/tokens", CreateProjectToken)
            .WithTags("Projects", "Access")
            .WithSummary("Create project token")
            .WithDescription("Creates a new project token")
            .WithRequestValidation<WebCreateProjectTokenRequest>()
            .RequireAction("token:create");

        app.MapDelete("/api/projects/{projectId}/tokens/{tokenId}", RemoveProjectToken)
            .WithTags("Projects", "Access")
            .WithSummary("Remove project token")
            .WithDescription("Removes a token from the project.")
            .RequireAction("token:delete");
    }

    private static async Task<Ok<WebGetProjectTokensResponse>> GetProjectTokens(
        [FromRoute] string projectId,
        IManageProjectTokensUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var members = await useCase.GetProjectTokens(
            ProjectId.FromValue(projectId),
            cancellationToken
        );

        return TypedResults.Ok(
            new WebGetProjectTokensResponse(members.Select(WebProjectToken.Map))
        );
    }

    private static async Task<Created<WebCreateProjectTokenResponse>> CreateProjectToken(
        [FromRoute] string projectId,
        [FromBody] WebCreateProjectTokenRequest command,
        IManageProjectTokensUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.CreateProjectToken(
            command.Map(ProjectId.FromValue(projectId)),
            cancellationToken
        );

        return TypedResults.Created(
            $"/api/projects/{projectId}/tokens/{result.Token.Id.Value}",
            WebCreateProjectTokenResponse.Map(result)
        );
    }

    private static async Task<NoContent> RemoveProjectToken(
        [FromRoute] string projectId,
        [FromRoute] string tokenId,
        IManageProjectTokensUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.DeleteProjectToken(
            ProjectId.FromValue(projectId),
            new ProjectTokenId(tokenId),
            cancellationToken
        );

        return TypedResults.NoContent();
    }
}
