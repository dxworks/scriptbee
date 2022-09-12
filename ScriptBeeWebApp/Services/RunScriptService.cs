using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class RunScriptService : IRunScriptService
{
    private readonly IFileModelService _fileModelService;
    private readonly IRunModelService _runModelService;
    private readonly IProjectModelService _projectModelService;
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IPluginRepository _pluginRepository;

    public RunScriptService(IFileModelService fileModelService, IRunModelService runModelService,
        IProjectModelService projectModelService, IHelperFunctionsFactory helperFunctionsFactory,
        IFileNameGenerator fileNameGenerator, IProjectFileStructureManager projectFileStructureManager,
        IPluginRepository pluginRepository)
    {
        _fileModelService = fileModelService;
        _runModelService = runModelService;
        _projectModelService = projectModelService;
        _helperFunctionsFactory = helperFunctionsFactory;
        _fileNameGenerator = fileNameGenerator;
        _projectFileStructureManager = projectFileStructureManager;
        _pluginRepository = pluginRepository;
    }

    public IScriptRunner? GetScriptRunner(string language)
    {
        return _pluginRepository.GetPlugin<IScriptRunner>(runner => runner.Language == language);
    }

    public IEnumerable<string> GetSupportedLanguages()
    {
        return _pluginRepository.GetPlugins<IScriptGeneratorStrategy>(_ => true).Select(strategy => strategy.Language);
    }

    public async Task<RunModel?> RunAsync(IScriptRunner scriptRunner, Project project, ProjectModel projectModel,
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

        try
        {
            // var runResults = await scriptRunner.RunAsync(project, runModel.Id, scriptContent);
            //
            // foreach (var (type, filePath) in runResults)
            // {
            //     if (type.Equals(RunResult.ConsoleType))
            //     {
            //         runModel.ConsoleOutputName = filePath;
            //     }
            //     else if (type.Equals(RunResult.FileType))
            //     {
            //         runModel.OutputFileNames.Add(filePath);
            //     }
            // }
            await _runModelService.UpdateDocument(runModel, cancellationToken);

            return runModel;
        }
        catch (Exception e)
        {
            runModel.Errors = e.Message;
            await _runModelService.UpdateDocument(runModel, cancellationToken);

            throw;
        }
    }
}
