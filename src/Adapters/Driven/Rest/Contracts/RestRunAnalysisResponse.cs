using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Rest.Contracts;

public class RestRunAnalysisResponse
{
    public required string Id { get; set; }
    public required string ProjectId { get; set; }
    public required string ScriptId { get; set; }
    public required string Status { get; set; }
    public required DateTimeOffset CreationDate { get; set; }

    public AnalysisInfo MapToAnalysisInfo()
    {
        return new AnalysisInfo(
            new AnalysisId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            new ScriptId(ScriptId),
            null,
            new AnalysisStatus(Status),
            [],
            [],
            CreationDate,
            null
        );
    }
}
