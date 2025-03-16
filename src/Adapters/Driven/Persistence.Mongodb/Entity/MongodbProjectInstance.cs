using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity;

public class MongodbProjectInstance : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string ProjectId { get; init; }
    public required string Url { get; init; }
    public DateTimeOffset CreationDate { get; init; }

    public InstanceInfo ToCalculationInstanceInfo()
    {
        return new InstanceInfo(
            new InstanceId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            Url,
            CreationDate
        );
    }

    public static MongodbProjectInstance From(InstanceInfo instanceInfo)
    {
        return new MongodbProjectInstance
        {
            Id = instanceInfo.Id.ToString(),
            ProjectId = instanceInfo.ProjectId.Value,
            Url = instanceInfo.Url,
            CreationDate = instanceInfo.CreationDate,
        };
    }
}
