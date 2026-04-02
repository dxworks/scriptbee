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
    public required string Type { get; set; } = MongodbScriptType.File;
    public required string FilePath { get; init; }
    public required MongodbScriptLanguage? ScriptLanguage { get; init; }
    public required IEnumerable<MongodbScriptParameter>? Parameters { get; init; }

    public required IEnumerable<string>? ChildrenIds { get; init; }

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
            ChildrenIds = null,
        };
    }

    public static MongodbScript From(ScriptFolder folder)
    {
        return new MongodbScript
        {
            Id = folder.Id.ToString(),
            ProjectId = folder.ProjectId.ToString(),
            Type = MongodbScriptType.Folder,
            FilePath = folder.File.Path,
            ChildrenIds = folder.ChildrenIds.Select(c => c.ToString()),
            ScriptLanguage = null,
            Parameters = null,
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

    public ScriptFolder ToFolder()
    {
        return new ScriptFolder(
            new ScriptId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            new ProjectStructureFile(FilePath),
            ChildrenIds?.Select(c => new ScriptId(c)) ?? []
        );
    }

    public ProjectStructureEntry ToProjectStructureEntry()
    {
        if (Type == MongodbScriptType.Folder)
        {
            return ToFolder();
        }

        return ToScript();
    }
}
