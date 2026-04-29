using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Analysis;

using TriggerAnalysisResult = OneOf<AnalysisInfo, InstanceDoesNotExistsError>;

public class TriggerAnalysisService(
    IGetProjectInstance getProjectInstance,
    ITriggerInstanceAnalysis triggerInstanceAnalysis,
    IAnalysisTracker analysisTracker
) : ITriggerAnalysisUseCase
{
    public async Task<TriggerAnalysisResult> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<TriggerAnalysisResult>>(
            async instanceInfo =>
            {
                var triggerResult = await triggerInstanceAnalysis.Trigger(
                    instanceInfo,
                    command.ScriptId,
                    cancellationToken
                );

                analysisTracker.Track(triggerResult.Id, triggerResult.ProjectId);

                return triggerResult;
            },
            error => Task.FromResult<TriggerAnalysisResult>(error)
        );
    }
}
