using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Service.Analysis;

public interface IProjectManager
{
    public IProject CreateProject(string projectId, string projectName);

    public void AddToGivenProject(
        Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string sourceName
    );

    public IProject GetProject();

    public void RemoveSourceEntries(string sourceName);
}
