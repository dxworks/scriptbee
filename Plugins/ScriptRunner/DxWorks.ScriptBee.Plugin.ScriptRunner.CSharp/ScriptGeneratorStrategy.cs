using System.Text;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public class ScriptGeneratorStrategy : IScriptGeneratorStrategy
{
    private const string StartComment =
        "// Only the code written in the ExecuteScript method will be executed";

    public string Language => "csharp";
    public string Extension => ".cs";

    public string ExtractValidScript(string script)
    {
        return script;
    }

    public string GenerateClassName(Type classType)
    {
        var className = GetTypeName(classType);
        var keyword = GetClassNameKeyword(classType);

        if (!string.IsNullOrEmpty(classType.Namespace))
        {
            return $"namespace {classType.Namespace};{Environment.NewLine}{Environment.NewLine}public {keyword} {className}";
        }

        return $"public {keyword} {className}";
    }

    public string GenerateClassName(
        Type classType,
        Type baseClassType,
        out HashSet<Type> baseClassGenericTypes
    )
    {
        var baseTypeName = GetTypeName(baseClassType, out baseClassGenericTypes);
        var className = GetTypeName(classType);
        var keyword = GetClassNameKeyword(classType);

        if (!string.IsNullOrEmpty(classType.Namespace))
        {
            return $"namespace {classType.Namespace};{Environment.NewLine}{Environment.NewLine}public {keyword} {className} : {baseTypeName}";
        }

        return $"public {keyword} {className} : {baseTypeName}";
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
        var fieldTypeName = GetTypeName(fieldType, out genericTypes);
        return $"    {fieldModifier} {fieldTypeName} {fieldName};";
    }

    public string GenerateProperty(
        string propertyModifier,
        Type propertyType,
        string propertyName,
        out HashSet<Type> genericTypes
    )
    {
        var propertyTypeName = GetTypeName(propertyType, out genericTypes);
        return $"    {propertyModifier} {propertyTypeName} {propertyName} {{ get; set; }}";
    }

    public string GenerateMethod(
        string methodModifier,
        Type methodType,
        string methodName,
        List<Tuple<Type, string>> methodParams,
        out HashSet<Type> genericTypes
    )
    {
        var stringBuilder = new StringBuilder();

        var methodTypeName = GetTypeName(methodType, out genericTypes);
        stringBuilder.Append($"    {methodModifier} {methodTypeName} {methodName}(");

        for (var i = 0; i < methodParams.Count; i++)
        {
            var tuple = methodParams[i];
            var type = GetTypeName(tuple.Item1, out var genericParamSet);

            foreach (var genericParam in genericParamSet)
            {
                genericTypes.Add(genericParam);
            }

            stringBuilder.Append($"{type} {tuple.Item2}");
            if (i != methodParams.Count - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.AppendLine(")");
        stringBuilder.AppendLine("    {");

        if (methodTypeName != "void")
        {
            stringBuilder.AppendLine("        return default;");
        }

        stringBuilder.AppendLine("    }");

        return stringBuilder.ToString();
    }

    public string GenerateModelDeclaration(string modelType) => "";

    public async Task<string> GenerateSampleCode()
    {
        return await RelativeFileContentProvider.GetFileContentAsync(
            "SampleCodes/CSharpSampleCode.txt"
        );
    }

    public string GenerateEmptyClass() => "";

    public async Task<string> GenerateImports()
    {
        return await RelativeFileContentProvider.GetFileContentAsync(
            "SampleCodes/CSharpImports.txt"
        );
    }

    public string GetStartComment() => StartComment;

    public string GetEndComment() => "";

    private static string GetPrimitiveType(string type)
    {
        return type switch
        {
            "System.Byte" or "Byte" => "byte",
            "System.SByte" or "SByte" => "sbyte",
            "System.Decimal" or "Decimal" => "decimal",
            "System.Double" or "Double" => "double",
            "System.Single" or "Single" => "float",
            "System.Int32" or "Int32" => "int",
            "System.UInt32" or "UInt32" => "uint",
            "System.Int64" or "Int64" => "long",
            "System.UInt64" or "UInt64" => "ulong",
            "System.Int16" or "Int16" => "short",
            "System.UInt16" or "UInt16" => "ushort",
            "System.String" or "String" => "string",
            "System.Char" or "Char" => "char",
            "System.Boolean" or "Boolean" => "bool",
            "System.Void" or "Void" => "void",
            _ => type,
        };
    }

    private static string GetTypeName(Type type, out HashSet<Type> genericTypes)
    {
        genericTypes = [];

        if (!type.IsGenericType)
        {
            return GetPrimitiveType(type.Name);
        }

        var stringBuilder = new StringBuilder();

        var name = type.Name;
        name = name[..^2];

        stringBuilder.Append(name);
        stringBuilder.Append('<');

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

        stringBuilder.Append('>');

        return stringBuilder.ToString();
    }

    private static string GetTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return GetPrimitiveType(type.Name);
        }

        var stringBuilder = new StringBuilder();

        var name = type.Name;
        name = name[..^2];

        stringBuilder.Append(name);
        stringBuilder.Append('<');

        for (var i = 0; i < type.GenericTypeArguments.Length; i++)
        {
            stringBuilder.Append('T');
            stringBuilder.Append(i + 1);

            if (i != type.GenericTypeArguments.Length - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.Append('>');

        return stringBuilder.ToString();
    }

    private static string GetClassNameKeyword(Type classType)
    {
        if (classType.IsEnum)
        {
            return "enum";
        }

        return classType is { IsValueType: true, IsPrimitive: false } ? "struct" : "class";
    }
}
