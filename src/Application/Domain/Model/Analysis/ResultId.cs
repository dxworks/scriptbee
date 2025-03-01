namespace ScriptBee.Domain.Model.Analysis;

public readonly record struct ResultId(Guid Value)
{
    public ResultId(string value)
        : this(Guid.Parse(value)) { }

    public override string ToString() => Value.ToString();
}
