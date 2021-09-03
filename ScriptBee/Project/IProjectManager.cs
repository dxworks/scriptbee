using System.Collections.Generic;
using ScriptBeePlugin;

namespace ScriptBee.Project
{
    public interface IProjectManager
    {
        public void AddToProject(Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary, string sourceName);

        public Project GetProject();

        public void RemoveSourceEntries(string sourceName);
    }
}