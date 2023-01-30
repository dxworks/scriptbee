# Writing Your First Python Script

## Prerequisites

In order to write Python scripts, you need to have a Python Script Runner plugin installed. The examples uses ScriptBee
Default Plugin Bundle

## Template

```python title="script.py"
project: Project

# start script

ConsoleWriteLine("Hello, Python!")

# end script
```

## Model Uploading

From project details section, upload a model files for the specific loader. In this example, we will
use [Honeydew](https://github.com/dxworks/honeydew) Loader.

After the model file is uploaded, load your model by clicking the `Load Files` button.

## Script Execution

```python title="script.py"
project: Project

# start script

repos = ContextGetValue(project.Context, "Repository", "honeydew")

ConsoleWriteLine("Repos: " + str(repos.Count))

projects = []

for pair in repos:
    repo = pair.Value
    repoProjects = repo.Projects

    for repoProject in repoProjects:
        projects.append({
            "FilePath": repoProject.FilePath,
            "Name": repoProject.Name,
            "FileCount": len(repoProject.Files),
        })


ConsoleWriteLine(ConvertJson(projects))
ConsoleWriteLine("It contained " + str(len(projects)) + " projects.")
ExportJson("output.json", projects)

# end script
```

The script above will print the number of repositories and projects in the model. It will also export the projects to a
json file.
It uses the classes offered by the Honeydew plugin that are stored in the context.
Using the helper functions from `DxWorks.ScriptBee.Plugin.Api.HelperFunctions`, we can easily convert the projects to
json and export them to a file.

If every is ok, you should see the results in the console and the file outputs section.  
