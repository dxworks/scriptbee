namespace ScriptBee.Analysis.Ports;

public interface IAllocateInstance
{
    Task<string> Allocate(string imageName, CancellationToken cancellationToken = default);
}
