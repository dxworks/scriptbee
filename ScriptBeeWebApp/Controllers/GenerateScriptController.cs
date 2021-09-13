using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenerateScriptController : ControllerBase
    {
        private readonly IFileContentProvider _fileContentProvider;
        private readonly IProjectManager _projectManager;
        private readonly ILoadersHolder _loadersHolder;

        public GenerateScriptController(IFileContentProvider fileContentProvider, IProjectManager projectManager,
            ILoadersHolder loadersHolder)
        {
            _fileContentProvider = fileContentProvider;
            _projectManager = projectManager;
            _loadersHolder = loadersHolder;
        }

        [HttpGet]
        public IActionResult GetSampleCode(IFormCollection formData)
        {
            if (!formData.TryGetValue("projectId", out var projectId))
            {
                return BadRequest("Missing project id");
            }

            if (!formData.TryGetValue("scriptType", out var scriptType))
            {
                return BadRequest("Missing script type");
            }

            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return NotFound($"Could not find project with id: {projectId}");
            }

            var classes = new List<object>();

            foreach (var (_, dictionary) in project.Context)
            {
                foreach (var (_, model) in dictionary)
                {
                    classes.Add(model);
                }
            }

            switch (scriptType)
            {
                case "python":
                {
                    var sampleCode =
                        new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider), _loadersHolder)
                            .GetSampleCode(classes);

                    var zipStream = CreateFileZipStream(sampleCode, ".py");
                    return File(zipStream, "application/octet-stream", "DummyPythonSampleCode.zip");
                }
                case "javascript":
                {
                    var sampleCode = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider),
                            _loadersHolder)
                        .GetSampleCode(classes);

                    var zipStream = CreateFileZipStream(sampleCode, ".js");
                    return File(zipStream, "application/octet-stream", "DummyJavascriptSampleCode.zip");
                }
                case "csharp":
                {
                    var sampleCode =
                        new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider), _loadersHolder)
                            .GetSampleCode(classes);

                    var zipStream = CreateFileZipStream(sampleCode, ".cs");

                    return File(zipStream, "application/octet-stream", "DummyCSharpSampleCode.zip");
                }
                default:
                {
                    return BadRequest($"Script type {scriptType} is not supported");
                }
            }
        }

        private Stream CreateFileZipStream(IList<SampleCodeFile> sampleCode, string extension)
        {
            var zipStream = new MemoryStream();
            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var sampleCodeFile in sampleCode)
                {
                    var zipArchiveEntry = zip.CreateEntry(sampleCodeFile.Name + extension);

                    using (StreamWriter writer = new StreamWriter(zipArchiveEntry.Open()))
                    {
                        writer.Write(sampleCodeFile.Content);
                    }
                }
            }

            zipStream.Position = 0;
            return zipStream;
        }
    }
}