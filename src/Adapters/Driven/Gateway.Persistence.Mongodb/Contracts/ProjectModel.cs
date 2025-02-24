using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;

namespace ScriptBee.Gateway.Persistence.Mongodb.Contracts;

public class ProjectModel : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string Name { get; init; }
    public DateTimeOffset CreationDate { get; init; }

    public Dictionary<string, List<FileData>> SavedFiles { get; set; } = new();
    public Dictionary<string, List<FileData>> LoadedFiles { get; set; } = new();

    public string? Linker { get; set; }
    public Run? LastRun { get; set; }

    public ProjectDetails ToProjectDetails()
    {
        return new ProjectDetails(ProjectId.FromValue(Id), Name, CreationDate);
    }

    public static ProjectModel From(ProjectDetails projectDetails)
    {
        return new ProjectModel
        {
            Id = projectDetails.Id.Value,
            Name = projectDetails.Name,
            CreationDate = projectDetails.CreationDate,
        };
    }
}
