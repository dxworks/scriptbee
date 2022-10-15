using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Data.Exceptions;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class RunScriptService : IRunScriptService
{
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPluginRepository _pluginRepository;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IRunModelService _runModelService;

    public RunScriptService(IFileModelService fileModelService, IFileNameGenerator fileNameGenerator,
        IGuidGenerator guidGenerator, IPluginRepository pluginRepository,
        IProjectFileStructureManager projectFileStructureManager, IProjectModelService projectModelService,
        IRunModelService runModelService)
    {
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _guidGenerator = guidGenerator;
        _pluginRepository = pluginRepository;
        _projectFileStructureManager = projectFileStructureManager;
        _projectModelService = projectModelService;
        _runModelService = runModelService;
    }

    public IEnumerable<string> GetSupportedLanguages()
    {
        return _pluginRepository.GetLoadedPluginManifests()
            .SelectMany(manifest => manifest.ExtensionPoints)
            .OfType<ScriptRunnerPluginExtensionPoint>()
            .Select(manifest => manifest.Language);
    }

    public async Task<Run> RunAsync(IProject project, ProjectModel projectModel, string language,
        string scriptFilePath, CancellationToken cancellationToken = default)
    {
        var runIndex = projectModel.LastRun?.Index + 1 ?? 0;

        var (scriptId, scriptContent) =
            await SaveScriptContentAsync(projectModel.Id, scriptFilePath, cancellationToken);

        var results = await RunScriptAsync(project, runIndex, language, scriptContent, cancellationToken);

        var run = new Run
        {
            Index = runIndex,
            Linker = projectModel.Linker,
            LoadedFiles = projectModel.LoadedFiles,
            ScriptId = scriptId,
            ScriptPath = scriptFilePath,
            Results = results
        };

        await SaveRunAsync(projectModel, run, cancellationToken);

        return run;
    }

    private async Task<SavedScriptContent> SaveScriptContentAsync(string projectId, string scriptFilePath,
        CancellationToken cancellationToken)
    {
        var scriptContent = await _projectFileStructureManager.GetFileContentAsync(projectId, scriptFilePath);
        if (scriptContent == null)
        {
            throw new ScriptFileNotFoundException(scriptFilePath);
        }

        var scriptId = await SaveStringContentToFile(scriptContent, cancellationToken);

        return new SavedScriptContent(scriptId, scriptContent);
    }

    private async Task<Guid> SaveStringContentToFile(string scriptContent, CancellationToken cancellationToken)
    {
        var byteArray = Encoding.ASCII.GetBytes(scriptContent);
        await using var stream = new MemoryStream(byteArray);

        var scriptId = _guidGenerator.GenerateGuid();

        await _fileModelService.UploadFileAsync(scriptId.ToString(), stream, cancellationToken);
        return scriptId;
    }

    private async Task<List<RunResult>> RunScriptAsync(IProject project, int runIndex, string language,
        string scriptContent, CancellationToken cancellationToken = default)
    {
        var scriptRunner = _pluginRepository.GetPlugin<IScriptRunner>(runner => runner.Language == language);

        if (scriptRunner is null)
        {
            throw new ScriptRunnerNotFoundException(language);
        }

        var scriptGeneratorStrategy =
            _pluginRepository.GetPlugin<IScriptGeneratorStrategy>(strategy => strategy.Language == language);

        if (scriptGeneratorStrategy is null)
        {
            throw new ScriptGenerationStrategyNotFoundException(language);
        }

        var resultCollector = new ResultCollector(_guidGenerator);

        var helperFunctionsContainer = CreateHelperFunctionsContainer(project, runIndex, resultCollector);

        await Task.WhenAll(helperFunctionsContainer.GetFunctions().Select(f => f.OnLoadAsync(cancellationToken)));

        try
        {
            var validScript = scriptGeneratorStrategy.ExtractValidScript(scriptContent);

            await scriptRunner.RunAsync(project, helperFunctionsContainer, validScript, cancellationToken);
        }
        catch (Exception e)
        {
            var runErrorId = await SaveStringContentToFile(e.Message, cancellationToken);

            resultCollector.Add(runErrorId.ToString(), RunResultDefaultTypes.RunError);
        }

        await Task.WhenAll(helperFunctionsContainer.GetFunctions()
            .Select(f => f.OnUnloadAsync(cancellationToken)));

        return resultCollector.GetResults();
    }

    private HelperFunctionsContainer CreateHelperFunctionsContainer(IProject project, int runIndex,
        IResultCollector resultCollector)
    {
        var helperFunctionSettings = new HelperFunctionsSettings(project.Id, runIndex.ToString());

        var helperFunctionService = new HelperFunctionsResultService(helperFunctionSettings, resultCollector,
            _fileModelService, _fileNameGenerator);

        var helperFunctionsEnumerable = _pluginRepository.GetPlugins<IHelperFunctions>(
            new List<(Type @interface, object instance)>
            {
                (typeof(IHelperFunctionsResultService), helperFunctionService)
            });

        return new HelperFunctionsContainer(helperFunctionsEnumerable);
    }

    private async Task SaveRunAsync(ProjectModel projectModel, Run run, CancellationToken cancellationToken)
    {
        await _runModelService.AddRun(projectModel.Id, run, cancellationToken);

        projectModel.LastRun = run;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);
    }

    private sealed record SavedScriptContent(Guid Id, string Content);
}
