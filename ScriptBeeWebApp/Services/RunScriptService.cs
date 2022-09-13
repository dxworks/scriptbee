using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class RunScriptService : IRunScriptService
{
    private readonly IFileModelService _fileModelService;
    private readonly IRunModelService _runModelService;
    private readonly IProjectModelService _projectModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IPluginRepository _pluginRepository;

    public RunScriptService(IFileModelService fileModelService, IRunModelService runModelService,
        IProjectModelService projectModelService, IFileNameGenerator fileNameGenerator,
        IProjectFileStructureManager projectFileStructureManager, IPluginRepository pluginRepository)
    {
        _fileModelService = fileModelService;
        _runModelService = runModelService;
        _projectModelService = projectModelService;
        _fileNameGenerator = fileNameGenerator;
        _projectFileStructureManager = projectFileStructureManager;
        _pluginRepository = pluginRepository;
    }

    public IEnumerable<string> GetSupportedLanguages()
    {
        return _pluginRepository.GetLoadedPlugins<ScriptRunnerPluginManifest>()
            .Select(manifest => manifest.Spec.Language);
    }

    public async Task<RunModel?> RunAsync(Project project, ProjectModel projectModel, string language,
        string scriptFilePath, CancellationToken cancellationToken = default)
    {
        var scriptContent =
            await _projectFileStructureManager.GetFileContentAsync(project.Id, scriptFilePath);
        if (scriptContent == null)
        {
            return null;
        }

        var scriptName = _fileNameGenerator.GenerateScriptName(project.Id, scriptFilePath);

        var byteArray = Encoding.ASCII.GetBytes(scriptContent);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFileAsync(scriptName, stream, cancellationToken);


        var loadedFiles = new Dictionary<string, List<string>>();

        foreach (var (loaderName, files) in projectModel.LoadedFiles)
        {
            loadedFiles[loaderName] = files;
        }

        projectModel.LastRunIndex++;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        var runModel = new RunModel
        {
            RunIndex = projectModel.LastRunIndex,
            ProjectId = project.Id,
            ScriptName = scriptName,
            Linker = projectModel.Linker,
            LoadedFiles = loadedFiles,
        };


        await _runModelService.CreateDocument(runModel, cancellationToken);

        var resultCollector = new ResultCollector();

        // todo create script runner using di
        // todo register IResultCollector to the di container

        var helperFunctionSettings = new HelperFunctionsSettings(project.Id, runModel.Id);

        var helperFunctionService = new HelperFunctionsResultService(helperFunctionSettings, resultCollector,
            _fileModelService, _fileNameGenerator);

        var helperFunctionsEnumerable = _pluginRepository.GetPlugins<IHelperFunctions>(
            new List<(Type @interface, object instance)>
            {
                (typeof(IHelperFunctionsResultService), helperFunctionService)
            });

        var helperFunctionsContainer = new HelperFunctionsContainer(helperFunctionsEnumerable);

        await Task.WhenAll(helperFunctionsContainer.GetFunctions()
            .Select(f => f.OnLoadAsync(cancellationToken)));

        var scriptRunner = _pluginRepository.GetPlugin<IScriptRunner>(runner => runner.Language == language);

        if (scriptRunner is null)
        {
            throw new Exception("Script runner not found");
        }

        var scriptGeneratorStrategy =
            _pluginRepository.GetPlugin<IScriptGeneratorStrategy>(strategy => strategy.Language == language);

        if (scriptGeneratorStrategy is null)
        {
            throw new Exception("Script generator strategy not found");
        }

        try
        {
            var validScript = scriptGeneratorStrategy.ExtractValidScript(scriptContent);

            await scriptRunner.RunAsync(project, helperFunctionsContainer, validScript, cancellationToken);
        }
        catch (Exception e)
        {
            runModel.Errors = e.Message;
            await _runModelService.UpdateDocument(runModel, cancellationToken);
            
        }

        await Task.WhenAll(helperFunctionsContainer.GetFunctions()
            .Select(f => f.OnUnloadAsync(cancellationToken)));

        foreach (var (filePath, type) in resultCollector.GetResults())
        {
            switch (type)
            {
                case RunResultDefaultTypes.ConsoleType:
                    runModel.ConsoleOutputName = filePath;
                    break;
                case RunResultDefaultTypes.FileType:
                    runModel.OutputFileNames.Add(filePath);
                    break;
            }
        }

        await _runModelService.UpdateDocument(runModel, cancellationToken);

        return runModel;
    }
}
