namespace ScriptBee.Domain.Model.Analysis;

public readonly record struct AnalysisId(Guid Value)
{
    public AnalysisId(string value)
        : this(Guid.Parse(value)) { }

    public override string ToString() => Value.ToString();
}
