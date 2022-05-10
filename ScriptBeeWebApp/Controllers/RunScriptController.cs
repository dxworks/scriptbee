using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HelperFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Config;
using ScriptBee.Models;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptRunners;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Extensions;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class RunScriptController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileModelService _fileModelService;
    private readonly IRunModelService _runModelService;
    private readonly IProjectModelService _projectModelService;
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IHelperFunctionsMapper _helperFunctionsMapper;

    public RunScriptController(IProjectManager projectManager,
        IProjectFileStructureManager projectFileStructureManager, IFileNameGenerator fileNameGenerator,
        IFileModelService fileModelService, IRunModelService runModelService,
        IProjectModelService projectModelService, IHelperFunctionsFactory helperFunctionsFactory,
        IHelperFunctionsMapper helperFunctionsMapper)
    {
        _projectManager = projectManager;
        _projectFileStructureManager = projectFileStructureManager;
        _fileNameGenerator = fileNameGenerator;
        _fileModelService = fileModelService;
        _runModelService = runModelService;
        _projectModelService = projectModelService;
        _helperFunctionsFactory = helperFunctionsFactory;
        _helperFunctionsMapper = helperFunctionsMapper;
    }

    [HttpPost]
    public async Task<IActionResult> RunScriptFromPath(RunScript arg, CancellationToken cancellationToken)
    {
        if (arg == null || string.IsNullOrEmpty(arg.projectId) || string.IsNullOrEmpty(arg.filePath))
        {
            return BadRequest("Invalid arguments!");
        }

        var scriptType = "";
        if (arg.filePath.EndsWith(".py"))
        {
            scriptType = "python";
        }
        else if (arg.filePath.EndsWith(".cs"))
        {
            scriptType = "csharp";
        }
        else if (arg.filePath.EndsWith(".js"))
        {
            scriptType = "javascript";
        }

        var scriptRunner = GetScriptRunner(scriptType);

        if (scriptRunner == null)
        {
            return BadRequest($"Script type {scriptType} is not supported");
        }

        var project = _projectManager.GetProject(arg.projectId);
        if (project == null)
        {
            return NotFound($"Could not find project with id: {arg.projectId}");
        }

        var scriptContent = await _projectFileStructureManager.GetFileContentAsync(arg.projectId, arg.filePath);
        if (scriptContent == null)
        {
            return NotFound($"File from {arg.filePath} not found");
        }

        var scriptName = _fileNameGenerator.GenerateScriptName(arg.projectId, arg.filePath);

        var byteArray = Encoding.ASCII.GetBytes(scriptContent);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFile(scriptName, stream, cancellationToken);

        var projectModel = await _projectModelService.GetDocument(arg.projectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {arg.projectId}");
        }

        var loadedFiles = new Dictionary<string, List<string>>();

        foreach (var (loaderName, files) in projectModel.LoadedFiles)
        {
            loadedFiles[loaderName] = files;
        }

        projectModel.LastRunIndex++;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        var runModel = new RunModel
        {
            RunIndex = projectModel.LastRunIndex,
            ProjectId = arg.projectId,
            ScriptName = scriptName,
            Linker = projectModel.Linker,
            LoadedFiles = loadedFiles,
        };

        await _runModelService.CreateDocument(runModel, cancellationToken);
        
        try
        {
            var runResults = await scriptRunner.Run(project, runModel.Id, scriptContent);

            foreach (var (type, filePath) in runResults)
            {
                if (type.Equals(RunResult.ConsoleType))
                {
                    runModel.ConsoleOutputName = filePath;
                }
                else if (type.Equals(RunResult.FileType))
                {
                    runModel.OutputFileNames.Add(filePath);
                }
            }
        }
        catch (Exception e)
        {
            runModel.Errors = e.Message;

            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                detail: $"Run script failed because {e}");
        }
        finally
        {
            await _runModelService.UpdateDocument(runModel, cancellationToken);
        }

        ReturnedRun returnedRun = new()
        {
            RunId = runModel.Id,
            RunIndex = runModel.RunIndex,
            ProjectId = runModel.ProjectId,
            Errors = runModel.Errors,
            ConsoleOutputName = runModel.ConsoleOutputName
        };

        List<OutputFile> outputFiles = new();

        foreach (var outputFileDatabaseName in runModel.OutputFileNames)
        {
            var (_, _, outputType, outputName) =
                _fileNameGenerator.ExtractOutputFileNameComponents(outputFileDatabaseName);
            OutputFile outputFile = new(outputName, outputType, outputFileDatabaseName);

            outputFiles.Add(outputFile);
        }

        returnedRun.OutputFiles = outputFiles;

        return Ok(returnedRun);
    }

    private IScriptRunner GetScriptRunner(string scriptType)
    {
        switch (scriptType)
        {
            case "python":
            {
                return new PythonScriptRunner(new PythonValidScriptExtractor(), _helperFunctionsFactory,
                    _helperFunctionsMapper);
            }
            case "javascript":
            {
                return new JavascriptScriptRunner(new JavascriptValidScriptExtractor(), _helperFunctionsFactory,
                    _helperFunctionsMapper);
            }
            case "csharp":
            {
                return new CSharpScriptRunner(new PluginPathReader(ConfigFolders.PathToPlugins),
                    _helperFunctionsFactory);
            }
            default:
            {
                return null;
            }
        }
    }
}