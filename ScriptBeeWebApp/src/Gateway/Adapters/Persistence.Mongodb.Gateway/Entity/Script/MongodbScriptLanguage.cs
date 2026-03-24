using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Persistence.Mongodb.Entity.Script;

public class MongodbScriptLanguage
{
    public required string Name { get; set; }
    public required string Extension { get; set; }

    public static MongodbScriptLanguage From(ScriptLanguage language)
    {
        return new MongodbScriptLanguage { Name = language.Name, Extension = language.Extension };
    }

    public ScriptLanguage ToScriptLanguage()
    {
        return new ScriptLanguage(Name, Extension);
    }
}
