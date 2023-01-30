# Writing Your First C# Script

## Prerequisites

In order to write C# scripts, you need to have a C# Script Runner plugin installed. The examples uses ScriptBee
Default Plugin Bundle

## Template

```csharp title="script.cs"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ScriptBee.ProjectContext;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using static DxWorks.ScriptBee.Plugin.Api.HelperFunctions;


// Only the code written in the ExecuteScript method will be executed

public class ScriptContent
{
    public void ExecuteScript(Project project)
    {
        ConsoleWriteLine("Hello, C#!");
    }
}

```

## Model Uploading

From project details section, upload a model files for the specific loader. In this example, we will
use [Honeydew](https://github.com/dxworks/honeydew) Loader.

After the model file is uploaded, load your model by clicking the `Load Files` button.

## Script Execution

```csharp title="script.cs"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ScriptBee.ProjectContext;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using static DxWorks.ScriptBee.Plugin.Api.HelperFunctions;


// Only the code written in the ExecuteScript method will be executed

public class ScriptContent
{
    public void ExecuteScript(Project project)
    {
        Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> models = project.Context.Models;

        Dictionary<string, ScriptBeeModel> repos = models[Tuple.Create("Repository", "honeydew")];

        ConsoleWriteLine("Repos: " + repos.Count);


        var projects = repos.Values
            .Select(repo => (repo as DxWorks.ScriptBee.Plugins.Honeydew.Models.RepositoryModel))
            .SelectMany(repo => repo.Projects)
            .Select(project => new
            {
                FilePath = project.FilePath,
                Name = project.Name,
                FileCount = project.Files.Count
            });

        ConsoleWriteLine(ConvertJson(projects));
        ConsoleWriteLine("It contained " + projects.Count() + " projects.");
        ExportJson("output.json", projects);
    }
}
```

The script above will print the number of repositories and projects in the model. It will also export the projects to a
json file.
It uses the classes offered by the Honeydew plugin that are stored in the context.
Using the helper functions from `DxWorks.ScriptBee.Plugin.Api.HelperFunctions`, we can easily convert the projects to
json and export them to a file.

If every is ok, you should see the results in the console and the file outputs section.  
