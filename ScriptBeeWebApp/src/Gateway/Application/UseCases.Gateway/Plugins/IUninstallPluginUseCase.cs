using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Plugins;

public interface IUninstallPluginUseCase
{
    Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> UninstallPluginAsync(
        UninstallPluginCommand command,
        CancellationToken cancellationToken = default
    );
}
