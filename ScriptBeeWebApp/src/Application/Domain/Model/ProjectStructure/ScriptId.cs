namespace ScriptBee.Domain.Model.ProjectStructure;

public readonly record struct ScriptId(Guid Value)
{
    public ScriptId(string value)
        : this(Guid.Parse(value)) { }

    public override string ToString() => Value.ToString();
}
