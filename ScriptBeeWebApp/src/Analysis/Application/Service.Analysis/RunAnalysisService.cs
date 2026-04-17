using System.Threading.Channels;
using DxWorks.ScriptBee.Plugin.Api;
using OneOf;
using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Common.Plugins;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class RunAnalysisService(
    IDateTimeProvider dateTimeProvider,
    IGuidProvider guidProvider,
    ICreateAnalysis createAnalysis,
    IGetScripts getScripts,
    IPluginRepository pluginRepository,
    Channel<RunScriptRequest> runScriptChannel,
    InstanceInformation instanceInformation
) : IRunAnalysisUseCase
{
    public async Task<AnalysisInfo> Run(
        RunAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var scriptResult = await getScripts.Get(command.ScriptId, cancellationToken);

        return await scriptResult.Match(
            async script => await Run(script, cancellationToken),
            async error =>
                await createAnalysis.Create(
                    AnalysisInfo.FailedToStart(
                        new AnalysisId(guidProvider.NewGuid()),
                        command.ProjectId,
                        instanceInformation.Id,
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
                        instanceInformation.Id,
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
                instanceInformation.Id,
                script.Id,
                dateTimeProvider.UtcNow()
            ),
            cancellationToken
        );

        await runScriptChannel.Writer.WriteAsync(
            new RunScriptRequest(scriptRunner, script, analysisInfo),
            cancellationToken
        );

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
