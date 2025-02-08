using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebGetProjectListResponse(List<WebGetProjectDetailsResponse> Projects)
{
    public static WebGetProjectListResponse Map(List<ProjectDetails> projectDetails)
    {
        return new WebGetProjectListResponse(projectDetails.Select(WebGetProjectDetailsResponse.Map).ToList());
    }
}
