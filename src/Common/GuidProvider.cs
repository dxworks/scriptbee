namespace ScriptBee.Common;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.CreateVersion7();
}
