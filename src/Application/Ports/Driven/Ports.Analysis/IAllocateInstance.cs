namespace ScriptBee.Ports.Analysis;

public interface IAllocateInstance
{
    Task<string> Allocate(string imageName, CancellationToken cancellationToken = default);
}
