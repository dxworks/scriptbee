using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Plugins;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

using InstallResult = Results<
    NoContent,
    BadRequest<ProblemDetails>,
    InternalServerError<ProblemDetails>
>;

public class InstallPluginEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/plugins", InstallPlugin)
            .WithTags("Plugins")
            .WithSummary("Install a plugin")
            .WithDescription("Installs a specific plugin version into the analysis service.")
            .WithRequestValidation<WebInstallPluginCommand>()
            .WithName("PluginsPost");
    }

    private static InstallResult InstallPlugin(
        HttpContext context,
        [FromBody] WebInstallPluginCommand command,
        IInstallPluginUseCase installPluginUseCase
    )
    {
        var result = installPluginUseCase.InstallPlugin(
            new PluginId(command.PluginId, new Version(command.Version))
        );

        return result.Match<InstallResult>(
            _ => TypedResults.NoContent(),
            error =>
                TypedResults.BadRequest(
                    context.ToProblemDetails(
                        "Plugin Installation Failed",
                        $"Invalid plugin version: {error.Id.Version} for {error.Id.Name}"
                    )
                ),
            error =>
                TypedResults.InternalServerError(
                    context.ToProblemDetails(
                        "Plugin Installation Failed",
                        $"An error occurred while installing plugin {error.Id.Name} version {error.Id.Version}"
                    )
                )
        );
    }
}
