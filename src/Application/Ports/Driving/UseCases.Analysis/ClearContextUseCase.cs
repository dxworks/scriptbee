namespace ScriptBee.UseCases.Analysis;

public interface IClearContextUseCase
{
    Task Clear(CancellationToken cancellationToken = default);
}
