using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Analysis;

public class DeleteAnalysisService(
    IGetAnalysis getAnalysis,
    IDeleteAnalysis deleteAnalysis,
    IFileModelService fileModelService
) : IDeleteAnalysisUseCase
{
    public async Task Delete(DeleteAnalysisCommand command, CancellationToken cancellationToken)
    {
        var result = await getAnalysis.GetById(command.AnalysisId, cancellationToken);

        await result.Match(
            async analysis =>
            {
                var fileIds = analysis.Results.Select(r => r.Id.ToFileId()).ToList();

                if (analysis.ScriptFileId is not null)
                {
                    fileIds.Add(analysis.ScriptFileId.Value);
                }

                if (fileIds.Count > 0)
                {
                    await fileModelService.DeleteFilesAsync(fileIds, cancellationToken);
                }

                await deleteAnalysis.DeleteById(command.AnalysisId, cancellationToken);
            },
            _ => Task.CompletedTask
        );
    }
}
