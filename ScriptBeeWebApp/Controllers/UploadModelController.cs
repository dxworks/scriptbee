using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
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

        public UploadModelController(IFolderWriter folderWriter, ILoadersHolder loadersHolder, IFileContentProvider fileContentProvider)
        {
            _folderWriter = folderWriter;
            _loadersHolder = loadersHolder;
            _fileContentProvider = fileContentProvider;
            _folderWriter.Initialize();
        }
        
        [HttpPost("fromfile")]
        public async Task<IActionResult> Post(IFormCollection formData)
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
            
            return Ok(dictionary.First().Value.First());
        }
    }
}