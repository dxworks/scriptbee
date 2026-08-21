namespace ScriptBee.Domain.Model.User;

public readonly record struct UserGroup(string Value)
{
    public override string ToString() => Value;
}
