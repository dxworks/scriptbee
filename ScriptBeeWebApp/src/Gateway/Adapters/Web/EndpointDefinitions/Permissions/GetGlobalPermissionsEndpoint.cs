using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Adapters.Auth;
using ScriptBee.Common.Web;
using ScriptBee.Service.Gateway;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.Permissions.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Permissions;

public class GetGlobalPermissionsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGlobalPermissionsUseCase, GlobalPermissionsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/permissions", GetGlobalPermissions)
            .WithTags("Permissions")
            .WithSummary("Get global permissions")
            .WithDescription("Get all the global permissions for the authenticated user.")
            .RequireAuthorization();
    }

    private static async Task<Ok<WebGetGlobalPermissionsResponse>> GetGlobalPermissions(
        CurrentUser currentUser,
        IGlobalPermissionsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var query = new GetGlobalPermissionsQuery(currentUser.Id, currentUser.Groups);
        var permissions = await useCase.GetGlobalPermissions(query, cancellationToken);

        return TypedResults.Ok(new WebGetGlobalPermissionsResponse(permissions));
    }
}
