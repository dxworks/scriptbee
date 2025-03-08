using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class RunAnalysisService(
    IDateTimeProvider dateTimeProvider,
    IGuidProvider guidProvider,
    ICreateAnalysis createAnalysis
) : IRunAnalysisUseCase
{
    public async Task<AnalysisInfo> Run(
        RunAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var analysisInfo = AnalysisInfo.Started(
            new AnalysisId(guidProvider.NewGuid()),
            command.ProjectId,
            command.ScriptId,
            dateTimeProvider.UtcNow()
        );

        await createAnalysis.Create(analysisInfo, cancellationToken);

        // TODO FIXIT(#20): implement the run of the script

        return analysisInfo;
    }
}
