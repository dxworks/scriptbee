using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Plugins;

using InstallPluginFromZipResult = OneOf<
    ProjectDetails,
    ProjectDoesNotExistsError,
    PluginManifestNotFoundError,
    PluginAlreadyExistsError,
    PluginInstallationError
>;

public interface IInstallPluginUseCase
{
    Task<
        OneOf<ProjectDetails, ProjectDoesNotExistsError, PluginInstallationError>
    > InstallPluginAsync(InstallPluginCommand command, CancellationToken cancellationToken);

    Task<InstallPluginFromZipResult> InstallPluginAsync(
        ProjectId projectId,
        Stream zipStream,
        CancellationToken cancellationToken
    );
}
