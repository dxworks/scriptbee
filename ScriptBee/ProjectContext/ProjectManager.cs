﻿using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;

namespace ScriptBee.ProjectContext;

// todo add tests
public class ProjectManager : IProjectManager
{
    private readonly Dictionary<string, Project> _projects = new Dictionary<string, Project>();

    public Project? CreateProject(string projectId, string projectName)
    {
        if (_projects.ContainsKey(projectId))
        {
            return null;
        }

        var project = new Project
        {
            Id = projectId,
            Name = projectName,
            CreationDate = DateTime.Now
        };

        _projects.Add(project.Id, project);
        return project;
    }

    public void LoadProject(ProjectModel projectModel)
    {
        if (_projects.ContainsKey(projectModel.Id))
        {
            return;
        }

        var project = new Project
        {
            Id = projectModel.Id,
            Name = projectModel.Name,
            CreationDate = projectModel.CreationDate
        };

        _projects.Add(project.Id, project);
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

            wantedProject.Context.SetModel(tuple, objectsDictionary);
        }
    }

    public Project? GetProject(string projectId)
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

        wantedProject.Context.RemoveLoaderEntries(sourceName);
    }

    public Dictionary<string, Project> GetAllProjects()
    {
        return _projects;
    }
}
