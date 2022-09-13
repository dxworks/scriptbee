using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Config;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class ProjectStructureService : IProjectStructureService
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILoadersHolder _loadersHolder;

    public ProjectStructureService(IProjectManager projectManager,
        IProjectFileStructureManager projectFileStructureManager, IPluginRepository pluginRepository,
        ILoadersHolder loadersHolder)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _pluginRepository = pluginRepository;
        _loadersHolder = loadersHolder;
    }

    public async Task<(string extension, string content)> GetSampleCodeAsync(string scriptType,
        CancellationToken cancellationToken = default)
    {
        var scriptGeneratorStrategy =
            _pluginRepository.GetPlugin<IScriptGeneratorStrategy>(manifest =>
                manifest.Language == scriptType);

        if (scriptGeneratorStrategy is null)
        {
            throw new Exception($"No plugin found for script type {scriptType}");
        }

        var sampleCode =
            await new SampleCodeGenerator(scriptGeneratorStrategy, _loadersHolder)
                .GenerateSampleCode(cancellationToken);

        return (scriptGeneratorStrategy.Extension, sampleCode);
    }

    public void GenerateModelClasses(string projectId)
    {
        var project = _projectManager.GetProject(projectId);

        if (project == null)
        {
            throw new ArgumentException("Project cannot be null!");
        }

        var classes = project.Context.GetClasses();

        // _pluginRepository.GetPlugin<IScriptGeneratorStrategy>(strategy=>strategy)

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
