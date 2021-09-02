using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.FolderManager;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadModelController : ControllerBase
    {
        private readonly IFolderWriter _folderWriter;

        public UploadModelController(IFolderWriter folderWriter)
        {
            _folderWriter = folderWriter;
            _folderWriter.Initialize();
        }
        
        [HttpPost("fromfile")]
        public IActionResult Post(IFormCollection formData)
        {
            if (!formData.TryGetValue("modelType", out var modelType))
            {
                return BadRequest("Missing model type");
            }
            
            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    _folderWriter.WriteToFile(modelType[0], file);
                }
            }
            return Ok();
        }
    }
}