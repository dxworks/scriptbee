using ScriptBee.Domain.Model.File;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebFileData(string Id, string Name)
{
    public static WebFileData Map(FileData data)
    {
        return new WebFileData(data.Id.ToString(), data.Name);
    }
}
