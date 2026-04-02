using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Artifacts.Mongodb.Entity.Script;

[BsonIgnoreExtraElements]
public class MongodbScript : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string ProjectId { get; init; }
    public required string Name { get; init; }
    public required string FilePath { get; init; }
    public required string AbsoluteFilePath { get; init; }
    public required MongodbScriptLanguage ScriptLanguage { get; init; }
    public required IEnumerable<MongodbScriptParameter> Parameters { get; init; } = [];

    public static MongodbScript From(Domain.Model.ProjectStructure.Script script)
    {
        return new MongodbScript
        {
            Id = script.Id.ToString(),
            ProjectId = script.ProjectId.ToString(),
            Type = MongodbScriptType.File,
            FilePath = script.File.Path,
            ScriptLanguage = MongodbScriptLanguage.From(script.ScriptLanguage),
            Parameters = script.Parameters.Select(MongodbScriptParameter.From),
        };
    }

    public Domain.Model.ProjectStructure.Script ToScript()
    {
        return new Domain.Model.ProjectStructure.Script(
            new ScriptId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            new ProjectStructureFile(FilePath),
            ScriptLanguage!.ToScriptLanguage(),
            Parameters!.Select(p => p.ToScriptParameter())
        );
    }
}
