using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Persistence.Mongodb.Entity.Analysis;

public class MongodbResultSummary
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required DateTimeOffset CreationDate { get; set; }

    public ResultSummary ToResultSummary()
    {
        return new ResultSummary(new ResultId(Id), Name, Type, CreationDate);
    }

    public static MongodbResultSummary From(ResultSummary resultSummary)
    {
        return new MongodbResultSummary
        {
            Id = resultSummary.Id.ToString(),
            Name = resultSummary.Name,
            Type = resultSummary.Type,
            CreationDate = resultSummary.CreationDate,
        };
    }
}
