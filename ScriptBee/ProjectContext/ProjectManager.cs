﻿using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBeePlugin;

namespace ScriptBee.ProjectContext
{
    public class ProjectManager : IProjectManager
    {
        private Dictionary<string, Project> _projects = new Dictionary<string, Project>();

        public void AddProject(string projectId)
        {
            _projects.Add(projectId, new Project
            {
                ProjectId = projectId
            });
        }

        public void RemoveProject(string projectId)
        {
            _projects.Remove(projectId);
        }


        public void AddToGivenProject(string projectId,
            Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary, string sourceName)
        {
            var wantedProject = _projects[projectId];

            foreach (var (exportedTypeName, objectsDictionary) in dictionary)
            {
                var tuple = new Tuple<string, string>(exportedTypeName, sourceName);
                if (wantedProject.Context.ContainsKey(tuple))
                {
                    wantedProject.Context[tuple] = objectsDictionary;
                }
                else
                {
                    wantedProject.Context.Add(tuple, objectsDictionary);
                }
            }
        }

        public Project GetProject(string projectId)
        {
            if (!_projects.TryGetValue(projectId, out var wantedProject))
            {
                return null;
            }

            return wantedProject;
        }

        public void RemoveSourceEntries(string projectId, string sourceName)
        {
            var wantedProject = _projects[projectId];

            var tuplesToBeRemoved = wantedProject.Context.Keys.Where(tuple => tuple.Item2.Equals(sourceName));
            foreach (var tuple in tuplesToBeRemoved)
            {
                wantedProject.Context.Remove(tuple);
            }
        }

        public Dictionary<string, Project> GetAllProjects()
        {
            return _projects;
        }
    }
}