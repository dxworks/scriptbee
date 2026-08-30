namespace ScriptBee.Common;

public sealed class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.CreateVersion7();
}
