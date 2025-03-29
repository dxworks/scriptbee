using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using TriggerAnalysisResult = OneOf<
    AnalysisInfo,
    ProjectDoesNotExistsError,
    InstanceDoesNotExistsError
>;

public class TriggerAnalysisService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    ITriggerInstanceAnalysis triggerInstanceAnalysis
) : ITriggerAnalysisUseCase
{
    public async Task<TriggerAnalysisResult> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match(
            async projectDetails =>
                await Trigger(
                    projectDetails,
                    command.InstanceId,
                    command.ScriptId,
                    cancellationToken
                ),
            error => Task.FromResult<TriggerAnalysisResult>(error)
        );
    }

    private async Task<TriggerAnalysisResult> Trigger(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match(
            async instanceInfo =>
                await Trigger(projectDetails, instanceInfo, scriptId, cancellationToken),
            error => Task.FromResult<TriggerAnalysisResult>(error)
        );
    }

    private async Task<TriggerAnalysisResult> Trigger(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#30): update project details with linkers and loaded files

        return await triggerInstanceAnalysis.Trigger(instanceInfo, scriptId, cancellationToken);
    }
}
