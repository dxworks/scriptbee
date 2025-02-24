namespace ScriptBee.Domain.Model.Calculation;

public sealed record CalculationInstanceId
{
    public string Value { get; }

    private CalculationInstanceId(string value)
    {
        Value = value;
    }

    public static CalculationInstanceId FromValue(string value)
    {
        return new CalculationInstanceId(value);
    }
}
