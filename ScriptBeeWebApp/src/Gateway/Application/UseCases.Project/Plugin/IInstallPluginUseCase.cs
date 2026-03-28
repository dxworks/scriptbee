using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Plugin;

public interface IInstallPluginUseCase
{
    Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> InstallPluginAsync(
        InstallPluginCommand command,
        CancellationToken cancellationToken = default
    );
}
