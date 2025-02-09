using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectResponse(string Id, string Name, DateTime CreationDate)
{
    public static WebCreateProjectResponse Map(ProjectDetails projectDetails)
    {
        return new WebCreateProjectResponse(
            projectDetails.Id.Value,
            projectDetails.Name,
            projectDetails.CreationDate);
    }
}
