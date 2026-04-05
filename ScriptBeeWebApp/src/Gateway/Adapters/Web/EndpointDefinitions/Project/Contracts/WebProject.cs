using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebProject(string Id, string Name, DateTimeOffset CreationDate)
{
    public static WebProject Map(ProjectDetails projectDetails)
    {
        return new WebProject(
            projectDetails.Id.Value,
            projectDetails.Name,
            projectDetails.CreationDate
        );
    }
}
