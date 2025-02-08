using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Project;
using ScriptBee.Gateway.Web.Extensions;
using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project;

public class DeleteProjectEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDeleteProjectUseCase, DeleteProjectService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app, IConfiguration configuration)
    {
        app.MapDelete("/api/scriptbee/projects/{projectId}", DeleteProject)
            .AddAuthorizationPolicy(AuthorizationRolesExtensions.DeleteProjectPolicy, configuration);
    }

    private static async Task<NoContent> DeleteProject(
        [FromRoute] string projectId,
        IDeleteProjectUseCase useCase, CancellationToken cancellationToken = default)
    {
        await useCase.DeleteProject(new DeleteProjectCommand(ProjectId.FromValue(projectId)), cancellationToken);

        return TypedResults.NoContent();
    }
}
