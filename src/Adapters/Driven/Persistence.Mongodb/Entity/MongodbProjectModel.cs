﻿using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity;

public class MongodbProjectModel : IDocument
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

    public static MongodbProjectModel From(ProjectDetails projectDetails)
    {
        return new MongodbProjectModel
        {
            Id = projectDetails.Id.Value,
            Name = projectDetails.Name,
            CreationDate = projectDetails.CreationDate,
        };
    }
}
