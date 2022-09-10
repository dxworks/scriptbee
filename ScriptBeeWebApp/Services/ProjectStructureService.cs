using System;
using System.Collections.Generic;
using System.IO;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Config;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Services;

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

        
        // todo
        // var pythonModelClasses =
        //     new SampleCodeGenerator(new PythonScriptGeneratorStrategy(_fileContentProvider), _loadersHolder)
        //         .GetSampleCode(classes);
        //
        // WriteSampleCodeFiles(pythonModelClasses, projectId, "python", ".py");
        //
        // var javascriptModelClasses = new SampleCodeGenerator(
        //         new JavascriptScriptGeneratorStrategy(_fileContentProvider),
        //         _loadersHolder)
        //     .GetSampleCode(classes);
        //
        // WriteSampleCodeFiles(javascriptModelClasses, projectId, "javascript", ".js");
        //
        // var csharpModelClasses =
        //     new SampleCodeGenerator(new CSharpScriptGeneratorStrategy(_fileContentProvider), _loadersHolder)
        //         .GetSampleCode(classes);

        // WriteSampleCodeFiles(csharpModelClasses, projectId, "csharp", ".cs");
    }

    private void WriteSampleCodeFiles(IEnumerable<SampleCodeFile> sampleCodeFiles, string projectId, string folderName,
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
