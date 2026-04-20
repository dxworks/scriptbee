using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Marketplace.Errors;

namespace ScriptBee.Plugins.Marketplace;

public interface IPluginUrlFetcher
{
    Task<OneOf<string, PluginNotFoundError, PluginVersionNotFoundError>> GetPluginUrl(
        PluginId pluginId,
        CancellationToken cancellationToken
    );
}
