using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.UseCases.Gateway.Plugins;
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
        app.MapPut("/api/projects/{projectId}/plugins/{pluginId}", InstallPlugin)
            .WithTags("Plugins")
            .WithSummary("Install a plugin from marketplace")
            .WithDescription(
                "Installs a specific version of a plugin from the marketplace into the project."
            );
        app.MapPost("/api/projects/{projectId}/plugins", UploadPlugin)
            .WithTags("Plugins")
            .WithSummary("Upload and install a plugin")
            .WithDescription("Uploads a plugin ZIP file and installs it into the project.");
    }

    private static async Task<
        Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>
    > InstallPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IInstallPluginUseCase installPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await installPluginUseCase.InstallPluginAsync(
            new InstallPluginCommand(
                ProjectId.FromValue(projectId),
                new PluginId(pluginId, new Version(version))
            ),
            cancellationToken
        );

        return result.Match<
            Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>
        >(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<
            Ok<ProjectDetails>,
            NotFound<ProblemDetails>,
            BadRequest<ProblemDetails>,
            InternalServerError<ProblemDetails>
        >
    > UploadPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromForm] IFormFile file,
        IInstallPluginUseCase installPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await installPluginUseCase.InstallPluginAsync(
            ProjectId.FromValue(projectId),
            file.OpenReadStream(),
            cancellationToken
        );

        return result.Match<
            Results<
                Ok<ProjectDetails>,
                NotFound<ProblemDetails>,
                BadRequest<ProblemDetails>,
                InternalServerError<ProblemDetails>
            >
        >(
            projectDetails => TypedResults.Ok(projectDetails),
            error => error.ToProblem(context),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
