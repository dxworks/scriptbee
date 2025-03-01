namespace ScriptBee.Domain.Model.Analysis;

public readonly record struct InstanceId(Guid Value)
{
    public InstanceId(string value)
        : this(Guid.Parse(value)) { }

    public override string ToString() => Value.ToString();
}
