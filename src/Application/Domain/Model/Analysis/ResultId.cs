namespace ScriptBee.Domain.Model.Analysis;

public sealed record ResultId
{
    public string Value { get; }

    private ResultId(string value)
    {
        Value = value;
    }

    public static ResultId FromValue(string value)
    {
        return new ResultId(value);
    }
}
