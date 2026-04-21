using System.Text;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

public class ScriptGeneratorStrategy : IScriptGeneratorStrategy
{
    public string Language => "python";
    public string Extension => ".py";

    public string ExtractValidScript(string script)
    {
        return ValidScriptExtractor.ExtractValidScript(script);
    }

    public string GenerateClassName(Type classType)
    {
        var className = GetTypeName(classType);
        return classType.IsEnum ? $"class {className}(Enum):" : $"class {className}:";
    }

    public string GenerateClassName(
        Type classType,
        Type baseClassType,
        out HashSet<Type> baseClassGenericTypes
    )
    {
        var baseTypeName = GetTypeName(baseClassType, out baseClassGenericTypes);
        var className = GetTypeName(classType);
        return classType.IsEnum
            ? $"class {className}(Enum):"
            : $"class {className}({baseTypeName}):";
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
        var fieldTypeName = GetTypeName(fieldType, out genericTypes);
        return $"    {fieldName}: {fieldTypeName}";
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

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"    def {methodName}(");

        for (var i = 0; i < methodParams.Count; i++)
        {
            var tuple = methodParams[i];
            stringBuilder.Append($"{tuple.Item2}");
            if (i != methodParams.Count - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.AppendLine("):");
        stringBuilder.AppendLine("        pass");

        return stringBuilder.ToString();
    }

    public string GenerateModelDeclaration(string modelType) => $"project: {modelType}";

    public Task<string> GenerateSampleCode() =>
        RelativeFileContentProvider.GetFileContentAsync("SampleCodes/PythonSampleCode.txt");

    public string GenerateEmptyClass() => "    pass";

    public Task<string> GenerateImports() => Task.FromResult("");

    public string GetStartComment() => ValidScriptDelimiters.PythonStartComment;

    public string GetEndComment() => ValidScriptDelimiters.PythonEndComment;

    private static string GetPrimitiveTypeName(string typeName)
    {
        return typeName switch
        {
            "decimal"
            or "System.Decimal"
            or "Decimal"
            or "double"
            or "System.Double"
            or "Double"
            or "float"
            or "System.Single"
            or "Single" => "float",
            "byte"
            or "System.Byte"
            or "Byte"
            or "sbyte"
            or "System.SByte"
            or "SByte"
            or "int"
            or "System.Int32"
            or "Int32"
            or "uint"
            or "System.UInt32"
            or "UInt32"
            or "short"
            or "System.Int16"
            or "Int16"
            or "ushort"
            or "System.UInt16"
            or "UInt16" => "int",
            "long" or "System.Int64" or "Int64" or "ulong" or "System.UInt64" or "UInt64" => "long",
            "char" or "System.Char" or "Char" or "string" or "System.String" or "String" => "str",
            "bool" or "System.Boolean" or "Boolean" => "bool",
            _ => typeName,
        };
    }

    private static string GetTypeName(Type type, out HashSet<Type> genericTypes)
    {
        genericTypes = [];

        if (!type.IsGenericType)
        {
            return GetPrimitiveTypeName(type.Name);
        }

        StringBuilder stringBuilder = new StringBuilder();

        var name = type.Name;
        name = name[..^2];

        stringBuilder.Append(name);
        stringBuilder.Append('[');

        for (var i = 0; i < type.GenericTypeArguments.Length; i++)
        {
            var genericType = type.GenericTypeArguments[i];

            genericTypes.Add(genericType);

            stringBuilder.Append(GetTypeName(genericType, out var nestedGenericTypes));

            foreach (var genType in nestedGenericTypes)
            {
                genericTypes.Add(genType);
            }

            if (i != type.GenericTypeArguments.Length - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.Append(']');

        return stringBuilder.ToString();
    }

    private static string GetTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return GetPrimitiveTypeName(type.Name);
        }

        var stringBuilder = new StringBuilder();

        var name = type.Name;
        name = name[..^2];

        stringBuilder.Append(name);
        stringBuilder.Append('[');

        for (var i = 0; i < type.GenericTypeArguments.Length; i++)
        {
            stringBuilder.Append('T');
            stringBuilder.Append(i + 1);

            if (i != type.GenericTypeArguments.Length - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.Append(']');

        return stringBuilder.ToString();
    }
}
