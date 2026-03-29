namespace ScriptBee.Rest.Contracts;

public class RestRunAnalysisResponse
{
    public required string Id { get; set; }
    public required string ProjectId { get; set; }
    public required string ScriptId { get; set; }
    public required string Status { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
}
