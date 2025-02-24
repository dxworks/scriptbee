namespace ScriptBee.Ports.Driven.Calculation;

public interface IAllocateInstance
{
    Task<string> Allocate(string imageName, CancellationToken cancellationToken = default);
}
