using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Service.Analysis;

public class ProjectManager(Project project) : IProjectManager
{
    public void AddToGivenProject(
        Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string sourceName
    )
    {
        foreach (var (exportedTypeName, objectsDictionary) in dictionary)
        {
            var tuple = new Tuple<string, string>(exportedTypeName, sourceName);

            project.Context.SetModel(tuple, objectsDictionary);
        }
    }

    public IProject GetProject()
    {
        return project;
    }

    public void RemoveSourceEntries(string sourceName)
    {
        project.Context.RemoveLoaderEntries(sourceName);
    }
}
