using System;
using System.Collections.Generic;
using System.IO;
using ScriptBee.Config;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBeeWebApp.Services;

public class ProjectStructureService : IProjectStructureService
{
    private readonly IFileContentProvider _fileContentProvider;
    private readonly IProjectManager _projectManager;
    private readonly ILoadersHolder _loadersHolder;
    private readonly IProjectFileStructureManager _projectFileStructureManager;

    public ProjectStructureService(IFileContentProvider fileContentProvider, IProjectManager projectManager,
        ILoadersHolder loadersHolder, IProjectFileStructureManager projectFileStructureManager)
    {
        _fileContentProvider = fileContentProvider;
        _projectManager = projectManager;
        _loadersHolder = loadersHolder;
        _projectFileStructureManager = projectFileStructureManager;
    }

    public void GenerateModelClasses(string projectId)
    {
        var project = _projectManager.GetProject(projectId);

        if (project == null)
        {
            throw new ArgumentException("Project cannot be null!");
        }

        var classes = project.Context.GetClasses();

        var pythonModelClasses =
            new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider), _loadersHolder)
                .GetSampleCode(classes);

        WriteSampleCodeFiles(pythonModelClasses, projectId, "python", ".py");

        var javascriptModelClasses = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider),
                _loadersHolder)
            .GetSampleCode(classes);

        WriteSampleCodeFiles(javascriptModelClasses, projectId, "javascript", ".js");

        var csharpModelClasses =
            new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider), _loadersHolder)
                .GetSampleCode(classes);

        WriteSampleCodeFiles(csharpModelClasses, projectId, "csharp", ".cs");
    }

    private void WriteSampleCodeFiles(IList<SampleCodeFile> sampleCodeFiles, string projectId, string folderName,
        string extension)
    {
        var deleteFolderPath = Path.Combine(ConfigFolders.GeneratedFolder, folderName);
        _projectFileStructureManager.DeleteFolder(projectId, deleteFolderPath);

        foreach (var sampleCodeFile in sampleCodeFiles)
        {
            var filePath = Path.Combine(ConfigFolders.GeneratedFolder, folderName, sampleCodeFile.Name + extension);
            _projectFileStructureManager.CreateFile(projectId, filePath, sampleCodeFile.Content);
        }
    }
}