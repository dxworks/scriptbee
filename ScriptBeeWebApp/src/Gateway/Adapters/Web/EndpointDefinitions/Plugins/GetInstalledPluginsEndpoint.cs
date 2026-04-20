using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.UseCases.Gateway.Plugins;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetInstalledPluginsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IInstallPluginUseCase, InstallPluginService>();
        services.AddSingleton<
            IGetInstalledPluginDetailsUseCase,
            GetInstalledPluginDetailsService
        >();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/plugins", GetInstalledPlugins).WithTags("Plugins");
        app.MapGet("/api/projects/{projectId}/plugins/{pluginId}", GetPlugin).WithTags("Plugins");
    }

    private static async Task<
        Results<Ok<WebInstalledPluginsResponse>, NotFound<ProblemDetails>>
    > GetInstalledPlugins(
        HttpContext context,
        [FromRoute] string projectId,
        IGetProject getProject,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(ProjectId.FromValue(projectId), cancellationToken);

        return result.Match<Results<Ok<WebInstalledPluginsResponse>, NotFound<ProblemDetails>>>(
            details =>
                TypedResults.Ok(
                    new WebInstalledPluginsResponse(
                        details.InstalledPlugins.Select(WebInstalledPlugin.Map)
                    )
                ),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<Ok<WebMarketplacePluginWithDetails>, NotFound<ProblemDetails>>
    > GetPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        IGetInstalledPluginDetailsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.Get(ProjectId.FromValue(projectId), pluginId, cancellationToken);

        return result.Match<Results<Ok<WebMarketplacePluginWithDetails>, NotFound<ProblemDetails>>>(
            plugin => TypedResults.Ok(WebMarketplacePluginWithDetails.Map(plugin)),
            error => error.ToProblem(context)
        );
    }
}
