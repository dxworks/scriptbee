using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Data.Exceptions;
using ScriptBeeWebApp.Repository;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class RunScriptService : IRunScriptService
{
    private readonly IFileModelService _fileModelService;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPluginRepository _pluginRepository;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IRunModelService _runModelService;
    private readonly IScriptsService _scriptsService;

    public RunScriptService(IFileModelService fileModelService, IGuidGenerator guidGenerator,
        IPluginRepository pluginRepository, IProjectFileStructureManager projectFileStructureManager,
        IProjectModelService projectModelService, IRunModelService runModelService, IScriptsService scriptsService)
    {
        _fileModelService = fileModelService;
        _guidGenerator = guidGenerator;
        _pluginRepository = pluginRepository;
        _projectFileStructureManager = projectFileStructureManager;
        _projectModelService = projectModelService;
        _runModelService = runModelService;
        _scriptsService = scriptsService;
    }

    // TODO: run by id instead of path
    public async Task<Run> RunAsync(IProject project, ProjectModel projectModel, string language,
        string scriptFilePath, CancellationToken cancellationToken = default)
    {
        var runIndex = projectModel.LastRun?.Index + 1 ?? 0;

        var (scriptId, scriptContent) =
            await SaveScriptContentAsync(projectModel.Id, scriptFilePath, cancellationToken);

        var results =
            await RunScriptAsync(project, scriptFilePath, runIndex, language, scriptContent, cancellationToken);

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

    private async Task<List<RunResult>> RunScriptAsync(IProject project, string scriptId, int runIndex, string language,
        string scriptContent, CancellationToken cancellationToken = default)
    {
        var scriptRunner = _pluginRepository.GetPlugin<IScriptRunner>(runner => runner.Language == language);

        if (scriptRunner is null)
        {
            throw new ScriptRunnerNotFoundException(language);
        }

        var resultCollector = new ResultCollector();

        var helperFunctionsContainer = CreateHelperFunctionsContainer(project, runIndex, resultCollector);

        await Task.WhenAll(helperFunctionsContainer.GetFunctions().Select(f => f.OnLoadAsync(cancellationToken)));

        var scriptResponse = await _scriptsService.GetScriptByIdAsync(scriptId, project.Id, cancellationToken);

        var parameters = scriptResponse.Match(
            response => response.Parameters.Select(p => new ScriptParameter
            {
                Name = p.Name,
                Type = p.Type,
                Value = p.Value
            }),
            _ => Enumerable.Empty<ScriptParameter>(),
            _ => Enumerable.Empty<ScriptParameter>()
        );

        try
        {
            await scriptRunner.RunAsync(project, helperFunctionsContainer, parameters, scriptContent,
                cancellationToken);
        }
        catch (Exception e)
        {
            var runErrorId = await SaveStringContentToFile(e.Message, cancellationToken);

            resultCollector.Add(runErrorId, runIndex, "RunError", RunResultDefaultTypes.RunError);
        }

        await Task.WhenAll(helperFunctionsContainer.GetFunctions()
            .Select(f => f.OnUnloadAsync(cancellationToken)));

        return resultCollector.GetResults();
    }

    private HelperFunctionsContainer CreateHelperFunctionsContainer(IProject project, int runIndex,
        IResultCollector resultCollector)
    {
        var helperFunctionSettings = new HelperFunctionsSettings(project.Id, runIndex);

        var helperFunctionService = new HelperFunctionsResultService(helperFunctionSettings, resultCollector,
            _fileModelService, _guidGenerator);

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
