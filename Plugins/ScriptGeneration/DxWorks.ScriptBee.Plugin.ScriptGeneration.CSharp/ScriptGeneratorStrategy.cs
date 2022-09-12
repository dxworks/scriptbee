using System.Text;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.CSharp;

public class ScriptGeneratorStrategy : IScriptGeneratorStrategy
{
    private const string StartComment = "// Only the code written in the ExecuteScript method will be executed";

    public string Language => "csharp";
    public string Extension => ".cs";

    public string ExtractValidScript(string script)
    {
        return script;
    }

    public string GenerateClassName(Type classType)
    {
        var className = GetTypeName(classType);
        return $"public class {className}";
    }

    public string GenerateClassName(Type classType, Type baseClassType, out HashSet<Type> baseClassGenericTypes)
    {
        var baseTypeName = GetTypeName(baseClassType, out baseClassGenericTypes);
        var className = GetTypeName(classType);
        return $"public class {className} : {baseTypeName}";
    }

    public string GenerateClassStart()
    {
        return "{";
    }

    public string GenerateClassEnd()
    {
        return "}";
    }

    public string GenerateField(string fieldModifier, Type fieldType, string fieldName,
        out HashSet<Type> genericTypes)
    {
        var fieldTypeName = GetTypeName(fieldType, out genericTypes);
        return $"    {fieldModifier} {fieldTypeName} {fieldName};";
    }

    public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName,
        out HashSet<Type> genericTypes)
    {
        var propertyTypeName = GetTypeName(propertyType, out genericTypes);
        return $"    {propertyModifier} {propertyTypeName} {propertyName} {{ get; set; }}";
    }

    public string GenerateMethod(string methodModifier, Type methodType, string methodName,
        List<Tuple<Type, string>> methodParams, out HashSet<Type> genericTypes)
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

    public string GenerateModelDeclaration(string modelType)
    {
        return "";
    }

    public async Task<string> GenerateSampleCode()
    {
        return await RelativeFileContentProvider.GetFileContentAsync("SampleCodes/CSharpSampleCode.txt");
    }

    public string GenerateEmptyClass()
    {
        return "";
    }

    public async Task<string> GenerateImports()
    {
        return await RelativeFileContentProvider.GetFileContentAsync("SampleCodes/CSharpImports.txt");
    }

    public string GetStartComment()
    {
        return StartComment;
    }

    public string GetEndComment()
    {
        return "";
    }

    private string GetPrimitiveType(string type)
    {
        switch (type)
        {
            case "System.Byte":
            case "Byte":
            {
                return "byte";
            }
            case "System.SByte":
            case "SByte":
            {
                return "sbyte";
            }
            case "System.Decimal":
            case "Decimal":
            {
                return "decimal";
            }
            case "System.Double":
            case "Double":
            {
                return "double";
            }
            case "System.Single":
            case "Single":
            {
                return "float";
            }
            case "System.Int32":
            case "Int32":
            {
                return "int";
            }
            case "System.UInt32":
            case "UInt32":
            {
                return "uint";
            }
            case "System.Int64":
            case "Int64":
            {
                return "long";
            }
            case "System.UInt64":
            case "UInt64":
            {
                return "ulong";
            }
            case "System.Int16":
            case "Int16":
            {
                return "short";
            }
            case "System.UInt16":
            case "UInt16":
            {
                return "ushort";
            }
            case "System.String":
            case "String":
            {
                return "string";
            }
            case "System.Char":
            case "Char":
            {
                return "char";
            }
            case "System.Boolean":
            case "Boolean":
            {
                return "bool";
            }
            case "System.Void":
            case "Void":
            {
                return "void";
            }
        }

        return type;
    }

    private string GetTypeName(Type type, out HashSet<Type> genericTypes)
    {
        genericTypes = new HashSet<Type>();

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

    private string GetTypeName(Type type)
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
}
