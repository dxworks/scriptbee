using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Projects;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;

namespace ScriptBee.Gateway.Persistence.Mongodb.Contracts;

public class ProjectModel : IDocument
{
    [BsonId] public string Id { get; set; } = null!;
    public string Name { get; set; } = "";
    public DateTime CreationDate { get; set; }

    public Dictionary<string, List<FileData>> SavedFiles { get; set; } = new();
    public Dictionary<string, List<FileData>> LoadedFiles { get; set; } = new();

    public string? Linker { get; set; }
    public Run? LastRun { get; set; }

    public static ProjectModel From(Project project, DateTime creationDate)
    {
        return new ProjectModel
        {
            Id = project.Id.Value,
            Name = project.Name,
            CreationDate = creationDate,
        };
    }
}
