using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Service.Analysis;

public interface IProjectManager
{
    public Project CreateProject(string projectId, string projectName);

    public void AddToGivenProject(
        Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string sourceName
    );

    public Project GetProject();

    public void RemoveSourceEntries(string sourceName);
}
