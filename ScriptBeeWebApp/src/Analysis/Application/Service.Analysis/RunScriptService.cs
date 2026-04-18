using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
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
    IProjectManager projectManager
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
                await UpdateAnalysisFileNotFound(request.AnalysisInfo, error, cancellationToken)
        );
    }

    private async Task RunScript(
        RunScriptRequest request,
        string content,
        CancellationToken cancellationToken
    )
    {
        var metadata = new HistoricalScriptMetadata(
            request.Script.File.Path,
            request.Script.ScriptLanguage.Name,
            request.Script.ScriptLanguage.Extension
        );

        var scriptFileId = await SaveStringContentToFile(content, metadata, cancellationToken);

        var analysisInfo = request.AnalysisInfo with { ScriptFileId = scriptFileId };
        await updateAnalysis.Update(analysisInfo, cancellationToken);

        var results = await RunScriptAsync(
            request.ScriptRunner,
            request.Script,
            content,
            cancellationToken
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
