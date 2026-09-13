using InstalledPlugin = ScriptBee.Domain.Model.Plugins.Plugin;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IGetInstalledPluginsUseCase
{
    IEnumerable<InstalledPlugin> Get();
}
