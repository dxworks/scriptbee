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
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptRunners;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Extensions;
using ScriptBeeWebApp.Models;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers
{
    [ApiControllerRoute]
    [ApiController]
    public class RunScriptController : ControllerBase
    {
        private readonly IHelperFunctionsMapper _helperFunctionsMapper;
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileStructureManager _projectFileStructureManager;
        private readonly IFileNameGenerator _fileNameGenerator;
        private readonly IFileModelService _fileModelService;
        private readonly IRunModelService _runModelService;
        private readonly IProjectModelService _projectModelService;
        private readonly IProjectStructureService _projectStructureService;

        public RunScriptController(IProjectManager projectManager, IHelperFunctionsMapper helperFunctionsMapper,
            IProjectFileStructureManager projectFileStructureManager, IFileNameGenerator fileNameGenerator,
            IFileModelService fileModelService, IRunModelService runModelService,
            IProjectModelService projectModelService, IProjectStructureService projectStructureService)
        {
            _projectManager = projectManager;
            _helperFunctionsMapper = helperFunctionsMapper;
            _projectFileStructureManager = projectFileStructureManager;
            _fileNameGenerator = fileNameGenerator;
            _fileModelService = fileModelService;
            _runModelService = runModelService;
            _projectModelService = projectModelService;
            _projectStructureService = projectStructureService;
        }

        [HttpPost("fromfile")]
        public async Task<IActionResult> RunScriptFileContent(IFormCollection formData)
        {
            if (!formData.TryGetValue("scriptType", out var scriptType))
            {
                return BadRequest("Missing script type");
            }

            var scriptRunner = GetScriptRunner(scriptType);

            if (scriptRunner == null)
            {
                return BadRequest($"Script type {scriptType} is not supported");
            }

            if (!formData.TryGetValue("projectId", out var projectId))
            {
                return BadRequest("Missing project id");
            }

            var project = _projectManager.GetProject(projectId);
            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            List<string> scriptContents = new List<string>();

            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    var scriptContent = await file.ReadFormFileContent();
                    scriptContents.Add(scriptContent);
                }
            }

            foreach (var scriptContent in scriptContents)
            {
                scriptRunner.Run(project, scriptContent);
            }

            return Ok();
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

            await _projectStructureService.AddToProjectStructure(project.Id, arg.filePath, cancellationToken);

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

            var runModel = new RunModel
            {
                ProjectId = arg.projectId,
                ScriptName = scriptName,
                Linker = projectModel.Linker,
                LoadedFiles = loadedFiles,
                // todo console output
                // todo output files
            };

            try
            {
                scriptRunner.Run(project, scriptContent);
            }
            catch (Exception e)
            {
                runModel.Errors = e.Message;

                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Run script failed because {e}");
            }
            finally
            {
                await _runModelService.CreateDocument(runModel, cancellationToken);
            }

            return Ok();
        }

        private IScriptRunner GetScriptRunner(string scriptType)
        {
            switch (scriptType)
            {
                case "python":
                {
                    return new PythonScriptRunner(_helperFunctionsMapper, new PythonValidScriptExtractor());
                }
                case "javascript":
                {
                    return new JavascriptScriptRunner(_helperFunctionsMapper, new JavascriptValidScriptExtractor());
                }
                case "csharp":
                {
                    return new CSharpScriptRunner(new PluginPathReader(ConfigFolders.PathToPlugins),
                        _helperFunctionsMapper);
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}