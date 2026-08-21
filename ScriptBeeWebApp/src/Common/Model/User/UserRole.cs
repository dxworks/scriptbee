namespace ScriptBee.Domain.Model.User;

public readonly record struct UserRole(string Value)
{
    public override string ToString() => Value;
}
