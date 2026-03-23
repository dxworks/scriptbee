using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebScriptLanguage(string Name, string Extension)
{
    public static WebScriptLanguage Map(ScriptLanguage language)
    {
        return new WebScriptLanguage(language.Name, language.Extension);
    }
}
