namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public sealed record WebGetProjectFilesResponse(
    IEnumerable<WebProjectFileNode> Data,
    int TotalCount,
    int Offset,
    int Limit
)
{
    public bool HasNextPage => Offset + Limit < TotalCount;
}
