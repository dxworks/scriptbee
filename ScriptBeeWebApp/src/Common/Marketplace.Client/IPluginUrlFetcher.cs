using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client;

public interface IPluginUrlFetcher
{
    OneOf<string, PluginNotFoundError, PluginVersionNotFoundError> GetPluginUrl(
        string pluginId,
        string version
    );
}
