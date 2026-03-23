using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectResponse(string Id, string Name, DateTimeOffset CreationDate)
{
    public static WebCreateProjectResponse Map(ProjectDetails projectDetails)
    {
        return new WebCreateProjectResponse(
            projectDetails.Id.Value,
            projectDetails.Name,
            projectDetails.CreationDate
        );
    }
}
