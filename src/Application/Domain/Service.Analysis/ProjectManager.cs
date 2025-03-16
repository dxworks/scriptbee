using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Service.Analysis;

public class ProjectManager(IDateTimeProvider dateTimeProvider) : IProjectManager
{
    private Project Project { get; set; } = null!;

    public Project CreateProject(string projectId, string projectName)
    {
        Project = new Project
        {
            Id = projectId,
            Name = projectName,
            CreationDate = dateTimeProvider.UtcNow(),
        };
        return Project;
    }

    public void AddToGivenProject(
        Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string sourceName
    )
    {
        foreach (var (exportedTypeName, objectsDictionary) in dictionary)
        {
            var tuple = new Tuple<string, string>(exportedTypeName, sourceName);

            Project.Context.SetModel(tuple, objectsDictionary);
        }
    }

    public Project GetProject()
    {
        return Project;
    }

    public void RemoveSourceEntries(string sourceName)
    {
        Project.Context.RemoveLoaderEntries(sourceName);
    }
}
