using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Plugins.Installer;

public interface IPluginZipProcessor
{
    Task<
        OneOf<
            PluginId,
            PluginManifestNotFoundError,
            PluginAlreadyExistsError,
            PluginInstallationError
        >
    > ProcessZipStream(ProjectId projectId, Stream zipStream, CancellationToken cancellationToken);
}
