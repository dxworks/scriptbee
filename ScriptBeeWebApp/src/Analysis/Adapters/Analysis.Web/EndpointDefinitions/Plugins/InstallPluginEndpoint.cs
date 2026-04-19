using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins;

using InstallResult = Results<
    NoContent,
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
        app.MapPost("/api/plugins", InstallPlugin)
            .WithRequestValidation<WebInstallPluginCommand>()
            .WithTags("Plugins");
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
