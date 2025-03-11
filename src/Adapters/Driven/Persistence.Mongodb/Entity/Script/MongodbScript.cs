using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity.Script;

public class MongodbScript : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string ProjectId { get; init; }
    public required string Name { get; init; }
    public required string FilePath { get; init; }
    public required string AbsoluteFilePath { get; init; }
    public required string ScriptLanguageName { get; init; }
    public required IEnumerable<MongodbScriptParameter> Parameters { get; init; }

    public static MongodbScript From(Domain.Model.ProjectStructure.Script script)
    {
        return new MongodbScript
        {
            Id = script.Id.ToString(),
            ProjectId = script.ProjectId.ToString(),
            Name = script.Name,
            FilePath = script.FilePath,
            AbsoluteFilePath = script.AbsoluteFilePath,
            ScriptLanguageName = script.ScriptLanguage.Name,
            Parameters = script.Parameters.Select(MongodbScriptParameter.From),
        };
    }
}
