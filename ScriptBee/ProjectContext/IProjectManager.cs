using System.Collections.Generic;
using ScriptBeePlugin;

namespace ScriptBee.ProjectContext
{
    public interface IProjectManager
    {
        public void AddProject(string projectId);

        public void RemoveProject(string projectId);

        public void AddToGivenProject(string projectId,
            Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary, string sourceName);

        public Project GetProject(string projectId);

        public Dictionary<string, Project> GetAllProjects();

        public void RemoveSourceEntries(string projectId, string sourceName);
    }
}