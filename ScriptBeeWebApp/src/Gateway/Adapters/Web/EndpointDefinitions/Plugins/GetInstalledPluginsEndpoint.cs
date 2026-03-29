using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.UseCases.Project.Plugin;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetInstalledPluginsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IInstallPluginUseCase, InstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/plugins", GetInstalledPlugins).WithTags("Plugins");
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
}
