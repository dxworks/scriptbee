using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBeeWebApp.Arguments;
using ScriptBeeWebApp.Config;
using ScriptBeeWebApp.FolderManager;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            if (!formData.TryGetValue("modelType", out var modelType))
            {
                return BadRequest("Missing model type");
            }

            var modelLoader = _loadersHolder.GetModelLoader(modelType[0]);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {modelType[0]} is not supported");
            }

            List<string> fileContents = new List<string>();

            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(ConfigFolders.PathToModels, modelType[0], file.FileName);
                    await _folderWriter.WriteToFile(filePath, file);
                    string fileContent = await _fileContentProvider.GetFileContentAsync(filePath);
                    fileContents.Add(fileContent);
                }
            }

            var dictionary = modelLoader.LoadModel(fileContents);

            _projectManager.AddToProject(dictionary, modelLoader.GetName());

            return Ok();
        }

        [HttpPost("frompath")]
        public async Task<IActionResult> UploadFromPath(ScriptLoaderArguments scriptLoaderArguments)
        {
            if (scriptLoaderArguments.modelType == null)
            {
                return BadRequest("Missing model type");
            }

            var modelLoader = _loadersHolder.GetModelLoader(scriptLoaderArguments.modelType);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {scriptLoaderArguments.modelType} is not supported");
            }

            List<string> fileContents = new List<string>();

            List<string> wrongPaths = new List<string>();

            foreach (var modelPath in scriptLoaderArguments.paths)
            {
                var filePath = Path.Combine(ConfigFolders.PathToModels, modelPath);
                try
                {
                    string fileContent = await _fileContentProvider.GetFileContentAsync(filePath);
                    fileContents.Add(fileContent);
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

            var dictionary = modelLoader.LoadModel(fileContents);

            _projectManager.AddToProject(dictionary, modelLoader.GetName());

            return Ok();
        }
    }
}