# Writing Your First Javascript Script

## Prerequisites

In order to write Javascript scripts, you need to have a Javascript Script Runner plugin installed. The examples uses
ScriptBee
Default Plugin Bundle

## Template

> Only the code written between `// start script` and `// end script` will be executed

```javascript title="script.js"
let project = new Project();

// start script

ConsoleWriteLine("Hello, JavaScript!");

// end script
```

### With Parameters

ScriptBee supports parameterized scripts. This means that you can create add parameters while creating a script or edit
them later.

> Make sure the script has the parameters defined before running it.

```javascript title="script.js"
let project = new Project();
let scriptParameters = new ScriptParameters();

// start script

ConsoleWriteLine("Hello, JavaScript!");
ConsoleWriteLine("Parameter: " + scriptParameters.MyParameter);

// end script
```

## Model Uploading

From project details section, upload a model files for the specific loader. In this example, we will
use [Honeydew](https://github.com/dxworks/honeydew) Loader.

After the model file is uploaded, load your model by clicking the `Load Files` button.

## Script Execution

```javascript title="script.cs"
let project = new Project();

// start script

var repos = ContextGetValue(project.Context, "Repository", "honeydew");

ConsoleWriteLine("Repos: " + repos.Count);

var projects = [];

var keys = Object.keys(repos)

for (var i = 0; i < keys.length; i++) {
    var key = keys[i];
    var repo = repos[key];

    var repoProjects = repo.Projects;

    for (var j = 0; j < repoProjects.Count; j++) {
        var repoProject = repoProjects[j];
        projects.push({
            FilePath: repoProject.FilePath,
            Name: repoProject.Name,
            FileCount: repoProject.Files.Count,
        });
    }
}


ConsoleWriteLine(ConvertJson(projects));
ConsoleWriteLine("It contained " + projects.length + " projects.");
ExportJson("output.json", projects);

// end script
```

The script above will print the number of repositories and projects in the model. It will also export the projects to a
json file.
It uses the classes offered by the Honeydew plugin that are stored in the context.
Using the helper functions from `DxWorks.ScriptBee.Plugin.Api.HelperFunctions`, we can easily convert the projects to
json and export them to a file.

If everything is ok, you should see the results in the console and the file outputs section.  
