namespace ScriptBee.UseCases.Gateway.Analysis;

public interface IDeleteAnalysisUseCase
{
    Task Delete(DeleteAnalysisCommand command, CancellationToken cancellationToken);
}
