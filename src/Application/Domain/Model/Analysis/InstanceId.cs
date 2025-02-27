namespace ScriptBee.Domain.Model.Analysis;

public sealed record InstanceId
{
    public Guid Value { get; }

    private InstanceId(Guid value)
    {
        Value = value;
    }

    public static InstanceId FromValue(string value)
    {
        return new InstanceId(Guid.Parse(value));
    }

    public static InstanceId FromGuid(Guid guid)
    {
        return new InstanceId(guid);
    }

    public override string ToString() => Value.ToString();
}
