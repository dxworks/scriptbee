namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectResponse(string Id, string Name)
{
    public static WebCreateProjectResponse Map(Domain.Model.Projects.Project project)
    {
        return new WebCreateProjectResponse(project.Id.Value, project.Name);
    }
}
