using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class ScriptModel : IDocument
{
    [BsonId] public string Id { get; set; } = null!;

    public string ProjectId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string AbsoluteFilePath { get; set; } = null!;
    public string ScriptLanguage { get; set; } = null!;
    public List<ScriptParameterModel> Parameters { get; set; } = new();
}
