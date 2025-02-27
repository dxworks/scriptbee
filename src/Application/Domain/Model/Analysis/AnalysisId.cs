namespace ScriptBee.Domain.Model.Analysis;

public sealed record AnalysisId
{
    public Guid Value { get; }

    private AnalysisId(Guid value)
    {
        Value = value;
    }

    public static AnalysisId FromGuid(Guid guid)
    {
        return new AnalysisId(guid);
    }
}
