# Project Creation

Use the "Create Project" Button to create a new project.

The project id is the slugified name of the project.

When a new project is created, a new folder with the project id is created in the `projects` folder.
In the project folder, two folders are generated `generated` and `src`. The `generated` folder is used to store the
generated files from the scripts. The `src` folder is used to store the scripts.

## Project Context

When a new project is created, an empty context is initialized. The context is used to store the model loaded by the
plugins and where the scripts are executed on.

For more information about the context, check the [context section](context.md).

## Project Interface

```csharp
public interface IProject
{
    public IContext Context { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
}
```
