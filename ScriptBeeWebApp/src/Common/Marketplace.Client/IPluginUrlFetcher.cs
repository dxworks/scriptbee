using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client;

public interface IPluginUrlFetcher
{
    Task<OneOf<string, PluginNotFoundError, PluginVersionNotFoundError>> GetPluginUrl(
        PluginId pluginId,
        CancellationToken cancellationToken
    );
}
