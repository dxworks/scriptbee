namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisErrorResult(string Title, string Message, string Severity)
{
    public const string Critical = "Critical";
    public const string Major = "Major";
    public const string Minor = "Minor";
}
