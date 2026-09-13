namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface ILinkContextUseCase
{
    Task Link(IEnumerable<string> linkerIds, CancellationToken cancellationToken);
}
