using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Microsoft.Extensions.Logging;
using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Plugins.Loader;

namespace ScriptBee.Service.Analysis;

public sealed class RunScriptService(
    ILoadFile loadFile,
    IUpdateAnalysis updateAnalysis,
    IFileModelService fileModelService,
    IDateTimeProvider dateTimeProvider,
    IGuidProvider guidProvider,
    IPluginRepository pluginRepository,
    IProjectManager projectManager,
    ILogger<RunScriptService> logger
) : IRunScriptService
{
    public async Task RunAsync(
        RunScriptRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var scriptContent = await loadFile.GetScriptContent(
            request.Script.ProjectId,
            request.Script.File.Path,
            cancellationToken
        );

        await scriptContent.Match(
            async content =>
            {
                await RunScript(request, content, cancellationToken);
            },
            async error =>
            {
                logger.LogWarning(
                    "Script file not found for analysis {AnalysisId}: {Error}",
                    request.AnalysisInfo.Id,
                    error
                );
                await UpdateAnalysisFileNotFound(request.AnalysisInfo, error, cancellationToken);
            }
        );
    }

    private async Task RunScript(
        RunScriptRequest request,
        string content,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug(
            "Saving script file snapshot for analysis {AnalysisId} (language: {Language})",
            request.AnalysisInfo.Id,
            request.Script.ScriptLanguage.Name
        );

        var metadata = new HistoricalScriptMetadata(
            request.Script.File.Path,
            request.Script.ScriptLanguage.Name,
            request.Script.ScriptLanguage.Extension
        );

        var scriptFileId = await SaveStringContentToFile(content, metadata, cancellationToken);

        var analysisInfo = request.AnalysisInfo with { ScriptFileId = scriptFileId };
        await updateAnalysis.Update(analysisInfo, cancellationToken);

        logger.LogDebug(
            "Running script for analysis {AnalysisId} with runner {Runner}",
            request.AnalysisInfo.Id,
            request.ScriptRunner.GetType().Name
        );

        var results = await RunScriptAsync(
            request.ScriptRunner,
            request.Script,
            content,
            cancellationToken
        );

        logger.LogDebug(
            "Script execution finished for analysis {AnalysisId} — {ResultCount} result(s) produced",
            request.AnalysisInfo.Id,
            results.Count
        );

        await updateAnalysis.Update(
            analysisInfo.Success(dateTimeProvider.UtcNow(), results),
            cancellationToken
        );
    }

    private async Task UpdateAnalysisFileNotFound(
        AnalysisInfo analysisInfo,
        FileDoesNotExistsError error,
        CancellationToken cancellationToken
    )
    {
        await updateAnalysis.Update(
            analysisInfo.Failed(dateTimeProvider.UtcNow(), error.ToString()),
            cancellationToken
        );
    }

    private async Task<FileId> SaveStringContentToFile<TMetadata>(
        string scriptContent,
        TMetadata? metadata,
        CancellationToken cancellationToken
    )
        where TMetadata : class
    {
        var byteArray = Encoding.ASCII.GetBytes(scriptContent);
        await using var stream = new MemoryStream(byteArray);

        var fileId = new FileId(guidProvider.NewGuid());

        await fileModelService.UploadFileAsync(fileId, stream, metadata, cancellationToken);
        return fileId;
    }

    private async Task<List<ResultSummary>> RunScriptAsync(
        IScriptRunner scriptRunner,
        Script script,
        string scriptContent,
        CancellationToken cancellationToken = default
    )
    {
        var resultCollector = new ResultCollector(dateTimeProvider);

        var helperFunctionsContainer = CreateHelperFunctionsContainer(resultCollector);

        await Task.WhenAll(
            helperFunctionsContainer.GetFunctions().Select(f => f.OnLoadAsync(cancellationToken))
        );

        try
        {
            var project = projectManager.GetProject();
            await scriptRunner.RunAsync(
                project,
                helperFunctionsContainer,
                script.Parameters,
                scriptContent,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Script runner threw an exception for script {ScriptPath} (language: {Language})",
                script.File.Path,
                script.ScriptLanguage.Name
            );

            var runErrorId = await SaveStringContentToFile<object>(
                e.Message,
                null,
                cancellationToken
            );

            resultCollector.Add(
                new ResultId(runErrorId.Value),
                "RunError",
                RunResultDefaultTypes.RunError
            );
        }

        await Task.WhenAll(
            helperFunctionsContainer.GetFunctions().Select(f => f.OnUnloadAsync(cancellationToken))
        );

        return resultCollector.GetResults();
    }

    private HelperFunctionsContainer CreateHelperFunctionsContainer(
        IResultCollector resultCollector
    )
    {
        var helperFunctionService = new HelperFunctionsResultService(
            resultCollector,
            fileModelService,
            guidProvider
        );

        var helperFunctionsEnumerable = pluginRepository.GetPlugins<IHelperFunctions>(
            new List<(Type @interface, object instance)>
            {
                (typeof(IHelperFunctionsResultService), helperFunctionService),
            }
        );

        return new HelperFunctionsContainer(helperFunctionsEnumerable);
    }
}
