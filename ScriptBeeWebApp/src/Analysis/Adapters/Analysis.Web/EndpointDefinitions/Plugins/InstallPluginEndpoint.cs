using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Service.Plugin;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins;

using InstallResult = Results<
    Ok<WebInstallPluginResponse>,
    BadRequest<ProblemDetails>,
    InternalServerError<ProblemDetails>
>;

public class InstallPluginEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IInstallPluginUseCase, InstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/plugins", InstallPlugin).WithRequestValidation<WebInstallPluginCommand>();
    }

    private static async Task<InstallResult> InstallPlugin(
        HttpContext context,
        [FromBody] WebInstallPluginCommand command,
        IInstallPluginUseCase installPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await installPluginUseCase.InstallPlugin(
            command.PluginId,
            command.Version,
            cancellationToken
        );
        return result.Match<InstallResult>(
            _ => TypedResults.Ok(new WebInstallPluginResponse(command.PluginId, command.Version)),
            error =>
                TypedResults.BadRequest(
                    context.ToProblemDetails(
                        "Plugin Installation Failed",
                        $"Invalid plugin version: {error.Version} for {error.Name}"
                    )
                ),
            error =>
                TypedResults.InternalServerError(
                    context.ToProblemDetails(
                        "Plugin Installation Failed",
                        $"An error occurred while installing plugin {error.Name} version {error.Version}"
                    )
                )
        );
    }
}
