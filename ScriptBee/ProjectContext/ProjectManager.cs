using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBeePlugin;

namespace ScriptBee.ProjectContext
{
    public class ProjectManager : IProjectManager
    {
        private Project _project = new Project();

        public void AddToProject(Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary, string sourceName)
        {
            foreach (var (exportedTypeName, objectsDictionary) in dictionary)
            {
                Tuple<string, string> tuple = new Tuple<string, string>(exportedTypeName,sourceName);
                if (_project.Context.ContainsKey(tuple))
                {
                    _project.Context[tuple] = objectsDictionary;
                }
                else
                {
                    _project.Context.Add(tuple,objectsDictionary);
                }
            }
        }

        public Project GetProject()
        {
            return _project;
        }

        public void RemoveSourceEntries(string sourceName)
        {
            var tuplesToBeRemoved = _project.Context.Keys.Where(tuple => tuple.Item2.Equals(sourceName));
            foreach (var tuple in tuplesToBeRemoved)
            {
                _project.Context.Remove(tuple);
            }
        }
    }
}