using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Config;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class ProjectStructureService : IProjectStructureService
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILoadersService _loadersService;

    public ProjectStructureService(IProjectManager projectManager,
        IProjectFileStructureManager projectFileStructureManager, IPluginRepository pluginRepository,
        ILoadersService loadersService)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _pluginRepository = pluginRepository;
        _loadersService = loadersService;
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

        var acceptedModules = _loadersService.GetAcceptedModules();

        var sampleCode =
            await new SampleCodeGenerator(scriptGeneratorStrategy, acceptedModules).GenerateSampleCode(
                cancellationToken);

        return (scriptGeneratorStrategy.Extension, sampleCode);
    }

    public async Task GenerateModelClasses(string projectId, CancellationToken cancellationToken = default)
    {
        var project = _projectManager.GetProject(projectId);

        if (project == null)
        {
            throw new ArgumentException("Project cannot be null!");
        }

        var classes = project.Context.GetClasses();
        var acceptedModules = _loadersService.GetAcceptedModules();

        var generatorStrategies = _pluginRepository.GetPlugins<IScriptGeneratorStrategy>();
        foreach (var generatorStrategy in generatorStrategies)
        {
            var generatedClasses = await
                new SampleCodeGenerator(generatorStrategy, acceptedModules)
                    .GetSampleCode(classes);

            WriteSampleCodeFiles(generatedClasses, projectId, generatorStrategy.Language, generatorStrategy.Extension);
        }
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
