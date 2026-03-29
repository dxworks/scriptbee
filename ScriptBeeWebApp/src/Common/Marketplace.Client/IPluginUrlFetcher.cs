using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client;

public interface IPluginUrlFetcher
{
    Task<OneOf<string, PluginNotFoundError, PluginVersionNotFoundError>> GetPluginUrl(
        string pluginId,
        string version,
        CancellationToken cancellationToken
    );
}
