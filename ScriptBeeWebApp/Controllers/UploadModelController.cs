using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Config;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBeeWebApp.Arguments;
using ScriptBeeWebApp.FolderManager;

namespace ScriptBeeWebApp.Controllers
{
    [ApiControllerRoute]
    [ApiController]
    public class UploadModelController : ControllerBase
    {
        private readonly IFolderWriter _folderWriter;

        private readonly ILoadersHolder _loadersHolder;

        private readonly IFileContentProvider _fileContentProvider;

        private readonly IProjectManager _projectManager;

        public UploadModelController(IFolderWriter folderWriter, ILoadersHolder loadersHolder,
            IFileContentProvider fileContentProvider, IProjectManager projectManager)
        {
            _folderWriter = folderWriter;
            _loadersHolder = loadersHolder;
            _fileContentProvider = fileContentProvider;
            _projectManager = projectManager;
            _folderWriter.Initialize();
        }

        [HttpPost("fromfile")]
        public async Task<IActionResult> UploadFromFile(IFormCollection formData)
        {
            if (!formData.TryGetValue("loaderName", out var loaderName))
            {
                return BadRequest("Missing loader name");
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

            var modelLoader = _loadersHolder.GetModelLoader(loaderName[0]);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {loaderName[0]} is not supported");
            }

            var fileStreams = new List<Stream>();

            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(ConfigFolders.PathToModels, loaderName[0], file.FileName);
                    await _folderWriter.WriteToFile(filePath, file);
                    fileStreams.Add(System.IO.File.OpenRead(filePath));
                }
            }

            var dictionary = modelLoader.LoadModel(fileStreams);
            
            _projectManager.AddToGivenProject(projectId, dictionary, modelLoader.GetName());

            return Ok();
        }

        [HttpPost("frompath")]
        public async Task<IActionResult> UploadFromPath(ScriptLoaderArguments scriptLoaderArguments)
        {
            if (scriptLoaderArguments.loaderName == null)
            {
                return BadRequest("Missing loader name");
            }

            if (scriptLoaderArguments.projectId == null)
            {
                return BadRequest("Missing project id");
            }

            var project = _projectManager.GetProject(scriptLoaderArguments.projectId);

            if (project == null)
            {
                return NotFound($"Could not find project with id: {scriptLoaderArguments.projectId}");
            }

            var modelLoader = _loadersHolder.GetModelLoader(scriptLoaderArguments.loaderName);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {scriptLoaderArguments.loaderName} is not supported");
            }

            var fileStreams = new List<Stream>();

            var wrongPaths = new List<string>();

            foreach (var modelPath in scriptLoaderArguments.paths)
            {
                var filePath = Path.Combine(ConfigFolders.PathToModels, modelPath);
                try
                {
                    fileStreams.Add(System.IO.File.OpenRead(filePath));
                }
                catch (Exception)
                {
                    wrongPaths.Add(modelPath);
                }
            }

            if (wrongPaths.Count != 0)
            {
                string badRequestMessage = "Could not load models from the following paths:\n";
                foreach (var wrongPath in wrongPaths)
                {
                    badRequestMessage = badRequestMessage + wrongPath + "\n";
                }

                return BadRequest(badRequestMessage);
            }

            var dictionary = modelLoader.LoadModel(fileStreams);

            _projectManager.AddToGivenProject(scriptLoaderArguments.projectId, dictionary, modelLoader.GetName());

            return Ok();
        }
    }
}