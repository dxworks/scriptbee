# Script Generator Plugin

ScriptBee has a default Bundle that contains script generators for C#, Python and JavaScript.

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
extensionPoints:
  - kind: ScriptGenerator
    entryPoint: Generator.dll
    version: 1.0.1
    language: csharp
    extension: ".cs"
```

- `kind`: The type of plugin
- `entryPoint`: The relative path to the DLL containing the implemented interfaces for the respective plugins.
- `version`: The version of the plugin
- `language`: The programming language of the script generator
- `extension`: The extension of the script

## Script Generator Interface

```csharp title="IScriptGenerator.cs"
public interface IScriptGeneratorStrategy : IPlugin
{
    public string Language { get; }
    
    public string Extension { get; }

    public string ExtractValidScript(string script);

    public string GenerateClassName(Type classType);

    public string GenerateClassName(Type classType, Type baseClassType, out HashSet<Type> baseClassGenericTypes);

    public string GenerateClassStart();

    public string GenerateClassEnd();

    public string GenerateField(string fieldModifier, Type fieldType, string fieldName,
        out HashSet<Type> genericTypes);

    public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName,
        out HashSet<Type> genericTypes);

    public string GenerateMethod(string methodModifier, Type methodType, string methodName,
        List<Tuple<Type, string>> methodParams, out HashSet<Type> genericTypes);

    public string GenerateModelDeclaration(string modelType);

    public Task<string> GenerateSampleCode();

    public string GenerateEmptyClass();

    public Task<string> GenerateImports();

    public string GetStartComment();

    public string GetEndComment();
}
```

### Example

ScriptBee's default C# script generator can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptGeneration/DxWorks.ScriptBee.Plugin.ScriptGeneration.CSharp/ScriptGeneratorStrategy.cs)

ScriptBee's default Python script generator can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptGeneration/DxWorks.ScriptBee.Plugin.ScriptGeneration.Python/ScriptGeneratorStrategy.cs)

ScriptBee's default JavaScript script generator can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptGeneration/DxWorks.ScriptBee.Plugin.ScriptGeneration.Javascript/ScriptGeneratorStrategy.cs)
