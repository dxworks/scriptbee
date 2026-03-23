namespace ScriptBee.Rest.Contracts;

public class RestRunAnalysisCommand
{
    public required string ProjectId { get; set; }
    public required string ScriptId { get; set; }
}
