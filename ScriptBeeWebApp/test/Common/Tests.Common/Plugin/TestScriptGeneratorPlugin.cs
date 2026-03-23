using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Tests.Common.Plugin;

internal class TestScriptGeneratorPlugin : IScriptGeneratorStrategy
{
    public string Language => "";
    public string Extension => "";

    public string ExtractValidScript(string script) => script;

    public string GenerateClassName(Type classType) => "";

    public string GenerateClassName(
        Type classType,
        Type baseClassType,
        out HashSet<Type> baseClassGenericTypes
    )
    {
        baseClassGenericTypes = [];
        return "";
    }

    public string GenerateClassStart() => "";

    public string GenerateClassEnd() => "";

    public string GenerateField(
        string fieldModifier,
        Type fieldType,
        string fieldName,
        out HashSet<Type> genericTypes
    )
    {
        genericTypes = [];
        return "";
    }

    public string GenerateProperty(
        string propertyModifier,
        Type propertyType,
        string propertyName,
        out HashSet<Type> genericTypes
    )
    {
        genericTypes = [];
        return "";
    }

    public string GenerateMethod(
        string methodModifier,
        Type methodType,
        string methodName,
        List<Tuple<Type, string>> methodParams,
        out HashSet<Type> genericTypes
    )
    {
        genericTypes = [];
        return "";
    }

    public string GenerateModelDeclaration(string modelType) => "";

    public Task<string> GenerateSampleCode() => Task.FromResult("");

    public string GenerateEmptyClass() => "";

    public Task<string> GenerateImports() => Task.FromResult("");

    public string GetStartComment() => "";

    public string GetEndComment() => "";
}
