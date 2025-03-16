namespace ScriptBee.UseCases.Analysis;

public interface ILinkContextUseCase
{
    Task Link(IEnumerable<string> linkerIds, CancellationToken cancellationToken);
}
