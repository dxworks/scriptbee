using DxWorks.ScriptBee.Plugin.Api;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Project.Structure;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class RunAnalysisService(
    IDateTimeProvider dateTimeProvider,
    IGuidProvider guidProvider,
    ICreateAnalysis createAnalysis,
    IGetScript getScript,
    ILoadFile loadFile,
    IPluginRepository pluginRepository
) : IRunAnalysisUseCase
{
    public async Task<AnalysisInfo> Run(
        RunAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var scriptResult = await getScript.Get(command.ScriptId, cancellationToken);

        return await scriptResult.Match(
            async script => await Run(script, cancellationToken),
            async error =>
                await createAnalysis.Create(
                    AnalysisInfo.FailedToStart(
                        new AnalysisId(guidProvider.NewGuid()),
                        command.ProjectId,
                        command.ScriptId,
                        dateTimeProvider.UtcNow(),
                        error.ToString()
                    ),
                    cancellationToken
                )
        );
    }

    private async Task<AnalysisInfo> Run(
        Script script,
        CancellationToken cancellationToken = default
    )
    {
        var scriptRunnerResult = GetScriptRunner(script.ScriptLanguage);

        return await scriptRunnerResult.Match(
            runner => Run(runner, script, cancellationToken),
            async error =>
                await createAnalysis.Create(
                    AnalysisInfo.FailedToStart(
                        new AnalysisId(guidProvider.NewGuid()),
                        script.ProjectId,
                        script.Id,
                        dateTimeProvider.UtcNow(),
                        error.ToString()
                    ),
                    cancellationToken
                )
        );
    }

    private async Task<AnalysisInfo> Run(
        IScriptRunner scriptRunner,
        Script script,
        CancellationToken cancellationToken = default
    )
    {
        var analysisInfo = await createAnalysis.Create(
            AnalysisInfo.Started(
                new AnalysisId(guidProvider.NewGuid()),
                script.ProjectId,
                script.Id,
                dateTimeProvider.UtcNow()
            ),
            cancellationToken
        );

        // TODO FIXIT(#20): implement the run of the script
        await loadFile.GetScriptContent(script.ProjectId, script.FilePath, cancellationToken);

        return analysisInfo;
    }

    private OneOf<IScriptRunner, ScriptRunnerNotFoundError> GetScriptRunner(
        ScriptLanguage scriptLanguage
    )
    {
        var scriptRunner = pluginRepository.GetPlugin<IScriptRunner>(runner =>
            runner.Language == scriptLanguage.Name
        );

        return scriptRunner is null
            ? new ScriptRunnerNotFoundError(scriptLanguage)
            : OneOf<IScriptRunner, ScriptRunnerNotFoundError>.FromT0(scriptRunner);
    }
}
