using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetProjectListResponse(IEnumerable<WebGetProjectDetailsResponse> Data)
{
    public static WebGetProjectListResponse Map(IEnumerable<ProjectDetails> projectDetails)
    {
        return new WebGetProjectListResponse(
            projectDetails.Select(p => new WebGetProjectDetailsResponse(
                p.Id.Value,
                p.Name,
                p.CreationDate
            ))
        );
    }
}
