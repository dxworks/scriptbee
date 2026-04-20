using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Plugins;

public interface IPluginPathProvider
{
    string GetPathToPlugins();

    string GetPathToPlugins(ProjectId projectId);
}
