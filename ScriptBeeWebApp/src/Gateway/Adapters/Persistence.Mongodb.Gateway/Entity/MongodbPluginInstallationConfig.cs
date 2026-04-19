using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Persistence.Mongodb.Entity;

public class MongodbPluginInstallationConfig
{
    public string PluginId { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public static MongodbPluginInstallationConfig From(PluginInstallationConfig config)
    {
        return new MongodbPluginInstallationConfig
        {
            PluginId = config.PluginId,
            Version = config.Version.ToString(),
        };
    }

    public PluginInstallationConfig ToPluginInstallationConfig()
    {
        return new PluginInstallationConfig(PluginId, new Version(Version));
    }
}
