using System.IO;
using System.IO.Compression;
using DummyPlugin;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenerateScriptController : ControllerBase
    {
        private readonly IFileContentProvider _fileContentProvider;

        public GenerateScriptController(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        [HttpGet("{modelType}/{scriptType}")]
        public IActionResult Get(string modelType, string scriptType)
        {
            switch (modelType)
            {
                case "dummy":
                {
                    switch (scriptType)
                    {
                        case "python":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new PythonStrategyGenerator(_fileContentProvider)).Generate(
                                    typeof(DummyModel));

                            var zipStream = CreateFileZipStream("script.py", generatedTemplate);

                            return File(zipStream, "application/octet-stream", "DummyPythonSampleCode.zip");
                        }
                        case "javascript":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new JavascriptStrategyGenerator(_fileContentProvider))
                                    .Generate(
                                        typeof(DummyModel));

                            var zipStream = CreateFileZipStream("script.js", generatedTemplate);

                            return File(zipStream, "application/octet-stream", "DummyJavascriptSampleCode.zip");
                        }
                        case "csharp":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new CSharpStrategyGenerator(_fileContentProvider)).Generate(
                                    typeof(DummyModel));

                            var zipStream = CreateFileZipStream("script.cs", generatedTemplate);

                            return File(zipStream, "application/octet-stream", "DummyCSharpSampleCode.zip");
                        }
                        default:
                        {
                            return BadRequest($"Script type {scriptType} is not supported");
                        }
                    }
                }
                default:
                {
                    return BadRequest($"Model type {modelType} is not supported");
                }
            }
        }

        private Stream CreateFileZipStream(string fileName, string fileContent)
        {
            var zipStream = new MemoryStream();

            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var zipArchiveEntry = zip.CreateEntry(fileName);

                using (StreamWriter writer = new StreamWriter(zipArchiveEntry.Open()))
                {
                    writer.Write(fileContent);
                }
            }

            zipStream.Position = 0;

            return zipStream;
        }
    }
}