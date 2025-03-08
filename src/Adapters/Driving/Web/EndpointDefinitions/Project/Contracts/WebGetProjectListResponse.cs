using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebGetProjectListResponse(IEnumerable<WebGetProjectDetailsResponse> Projects)
{
    public static WebGetProjectListResponse Map(IEnumerable<ProjectDetails> projectDetails)
    {
        return new WebGetProjectListResponse(
            projectDetails.Select(WebGetProjectDetailsResponse.Map)
        );
    }
}
