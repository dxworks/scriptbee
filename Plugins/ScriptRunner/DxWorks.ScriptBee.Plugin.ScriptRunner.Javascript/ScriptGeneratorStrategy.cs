using System.Text;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

public class ScriptGeneratorStrategy : IScriptGeneratorStrategy
{
    public string Language => "javascript";
    public string Extension => ".js";

    public string ExtractValidScript(string script)
    {
        return ValidScriptExtractor.ExtractValidScript(script);
    }

    public string GenerateClassName(Type classType)
    {
        var className = GetTypeName(classType);

        return $"class {className}";
    }

    public string GenerateClassName(
        Type classType,
        Type baseClassType,
        out HashSet<Type> baseClassGenericTypes
    )
    {
        baseClassGenericTypes = [];

        var className = GetTypeName(classType);
        var baseClassName = GetTypeName(baseClassType);

        return $"class {className} extends {baseClassName}";
    }

    public string GenerateClassStart() => "{";

    public string GenerateClassEnd() => "}";

    public string GenerateField(
        string fieldModifier,
        Type fieldType,
        string fieldName,
        out HashSet<Type> genericTypes
    )
    {
        genericTypes = [];

        var fieldTypeName = GetTypeName(fieldType);
        return fieldType.IsEnum
            ? $"    {fieldName} = 0;"
            : $"    {fieldName} = {GetFieldInitializationValue(fieldTypeName)};";
    }

    public string GenerateProperty(
        string propertyModifier,
        Type propertyType,
        string propertyName,
        out HashSet<Type> genericTypes
    )
    {
        return GenerateField(propertyModifier, propertyType, propertyName, out genericTypes);
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

        var methodTypeName = GetTypeName(methodType);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"    {methodName}: function(");

        for (var i = 0; i < methodParams.Count; i++)
        {
            var tuple = methodParams[i];
            stringBuilder.Append($"{tuple.Item2}");
            if (i != methodParams.Count - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.AppendLine(")");
        stringBuilder.AppendLine("    {");

        if (methodTypeName != "void" && methodTypeName != "Void")
        {
            if (methodType.IsEnum)
            {
                stringBuilder.AppendLine("        return 0;");
            }
            else
            {
                stringBuilder.AppendLine(
                    $"        return {GetFieldInitializationValue(methodTypeName)};"
                );
            }
        }

        stringBuilder.AppendLine("    }");

        return stringBuilder.ToString();
    }

    public string GenerateModelDeclaration(string modelType) => $"let project = new {modelType}();";

    public async Task<string> GenerateSampleCode() =>
        await RelativeFileContentProvider.GetFileContentAsync(
            "SampleCodes/JavascriptSampleCode.txt"
        );

    public string GenerateEmptyClass() => "";

    public Task<string> GenerateImports() => Task.FromResult("");

    public string GetStartComment() => ValidScriptDelimiters.JavascriptStartComment;

    public string GetEndComment() => ValidScriptDelimiters.JavascriptEndComment;

    private static string GetFieldInitializationValue(string fieldType)
    {
        return fieldType switch
        {
            "byte"
            or "System.Byte"
            or "Byte"
            or "sbyte"
            or "System.SByte"
            or "SByte"
            or "decimal"
            or "System.Decimal"
            or "Decimal"
            or "double"
            or "System.Double"
            or "Double"
            or "float"
            or "System.Single"
            or "Single"
            or "int"
            or "System.Int32"
            or "Int32"
            or "uint"
            or "System.UInt32"
            or "UInt32"
            or "long"
            or "System.Int64"
            or "Int64"
            or "ulong"
            or "System.UInt64"
            or "UInt64"
            or "short"
            or "System.Int16"
            or "Int16"
            or "ushort"
            or "System.UInt16"
            or "UInt16" => "0",
            "char" or "System.Char" or "Char" or "string" or "System.String" or "String" => "\'\'",
            "bool" or "System.Boolean" or "Boolean" => "true",
            _ => $"new {fieldType}()",
        };
    }

    private static string GetTypeName(Type type)
    {
        var name = type.Name;

        if (type.IsGenericType)
        {
            name = name[..^2];
        }

        return name;
    }
}
