﻿using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Contracts;

public class ProjectInstance : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string ProjectId { get; init; }
    public required string Url { get; init; }
    public DateTimeOffset CreationDate { get; init; }

    public InstanceInfo ToCalculationInstanceInfo()
    {
        return new InstanceInfo(
            InstanceId.FromValue(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            Url,
            CreationDate
        );
    }

    public static ProjectInstance From(InstanceInfo instanceInfo)
    {
        return new ProjectInstance
        {
            Id = instanceInfo.Id.ToString(),
            ProjectId = instanceInfo.ProjectId.Value,
            Url = instanceInfo.Url,
            CreationDate = instanceInfo.CreationDate,
        };
    }
}
