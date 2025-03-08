namespace ScriptBee.Domain.Model.Analysis;

public readonly record struct AnalysisStatus(string Value)
{
    public static AnalysisStatus Started { get; } = new("Started");
    public static AnalysisStatus Running { get; } = new("Running");
    public static AnalysisStatus Finished { get; } = new("Finished");

    public override string ToString() => Value;
}
