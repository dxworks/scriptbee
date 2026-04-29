using System.ComponentModel;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

[Description("Represents detailed project information, including saved and loaded files.")]
public record WebProjectDetails(
    string Id,
    string Name,
    DateTimeOffset CreationDate,
    IDictionary<string, IEnumerable<WebFileData>> SavedFiles,
    IDictionary<string, IEnumerable<WebFileData>> LoadedFiles,
    IEnumerable<string> Linkers
)
{
    public static WebProjectDetails Map(ProjectDetails projectDetails)
    {
        return new WebProjectDetails(
            projectDetails.Id.Value,
            projectDetails.Name,
            projectDetails.CreationDate,
            projectDetails.SavedFiles.ToDictionary(
                x => x.Key,
                x => x.Value.Select(WebFileData.Map)
            ),
            projectDetails.LoadedFiles.ToDictionary(
                x => x.Key,
                x => x.Value.Select(WebFileData.Map)
            ),
            projectDetails.Linkers
        );
    }
}
