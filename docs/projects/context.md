# Project Context

The context is used to store the model loaded by the [loader plugins](../plugins/loader.md) and where the scripts are
executed on.
When a new project is created, an empty context is initialized.

The [linker plugins](../plugins/linker.md) are run on the context to create different connections between the entities.

## Models

The models are stored in a dictionary with the following structure:

```csharp
public Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> Models { get; set; }
```

The idea is that each loader plugin will parse the input files in order to convert them into a model that will be stored
in the context along with other models.

The key of the dictionary is a tuple of string identifiers. The first string is the name of the exported type from the
loader, and the second string is the name of the plugin.

The value of the first dictionary is a another dictionary that contains the exported entities. The key of the second
dictionary (entities dictionary) is a unique identifier set by the plugin, the actual value is not necessary relevant to
ScriptBee.

## ScriptBee Model Entity

The ScriptBeeModel is a base entity that every entity model from the plugins must inherit from.
It extends the `ExpandoObject` class from [Westwind Utilities](https://github.com/rickstrahl/westwind.utilities), so it
can be used as a dynamic object. This allows to get and set properties dynamically in the scripts, without having to
define them in advance.

```csharp
public class ScriptBeeModel : Expando
{
    public bool Ignored { get; set; }
}
```

## Context Interface

This is the interface of the context.

```csharp
public interface IContext
{
    public Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> Models { get; set; }

    public Dictionary<Tuple<string, string>, Dictionary<string, string>> Tags { get; set; }

    public void SetModel(Tuple<string, string> tuple, Dictionary<string, ScriptBeeModel> objectsDictionary);
   
    public void RemoveLoaderEntries(string sourceName);

    public List<object> GetClasses();

    public void Clear();
}
```

## Reload Context

When restarting the web server, the context needs to be reloaded before running any scripts.
This can be done using the "Reload Context" button from the project details page.
