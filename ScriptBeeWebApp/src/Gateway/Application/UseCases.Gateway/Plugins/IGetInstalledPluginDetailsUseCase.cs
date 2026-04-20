using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Plugins;

public interface IGetInstalledPluginDetailsUseCase
{
    Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> Get(
        ProjectId projectId,
        string pluginId,
        CancellationToken cancellationToken
    );
}
