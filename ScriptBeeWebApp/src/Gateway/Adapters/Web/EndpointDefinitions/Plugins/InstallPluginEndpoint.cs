using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.UseCases.Project.Plugin;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class InstallPluginEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IInstallPluginUseCase, InstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/projects/{projectId}/plugins/{pluginId}", InstallPlugin);
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> InstallPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IInstallPluginUseCase installPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await installPluginUseCase.InstallPluginAsync(
            new InstallPluginCommand(ProjectId.FromValue(projectId), pluginId, version),
            cancellationToken
        );

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context)
        );
    }
}
