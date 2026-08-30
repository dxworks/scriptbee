namespace ScriptBee.Domain.Model.Project;

public readonly record struct ProjectTokenId(string Value)
{
    public override string ToString() => Value;
}
