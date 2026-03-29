using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.UseCases.Project.Plugin;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class UninstallPluginEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUninstallPluginUseCase, UninstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}/plugins/{pluginId}", UninstallPlugin)
            .WithTags("Plugins");
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> UninstallPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IUninstallPluginUseCase uninstallPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await uninstallPluginUseCase.UninstallPluginAsync(
            new UninstallPluginCommand(ProjectId.FromValue(projectId), pluginId, version),
            cancellationToken
        );

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context)
        );
    }
}
