using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;

namespace ScriptBee.Gateway.Persistence.Mongodb.Contracts;

public class ProjectInstance : IDocument
{
    [BsonId] public required string Id { get; set; }
    public required string ProjectId { get; init; }
    public required string Url { get; init; }
    public DateTimeOffset CreationDate { get; init; }

    public CalculationInstanceInfo ToCalculationInstanceInfo()
    {
        return new CalculationInstanceInfo(CalculationInstanceId.FromValue(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId), Url, CreationDate);
    }

    public static ProjectInstance From(CalculationInstanceInfo instanceInfo)
    {
        return new ProjectInstance
        {
            Id = instanceInfo.Id.Value,
            ProjectId = instanceInfo.ProjectId.Value,
            Url = instanceInfo.Url,
            CreationDate = instanceInfo.CreationDate,
        };
    }
}
