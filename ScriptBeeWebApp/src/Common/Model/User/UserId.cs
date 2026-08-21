namespace ScriptBee.Domain.Model.User;

public readonly record struct UserId(string Value)
{
    public override string ToString() => Value;
}
