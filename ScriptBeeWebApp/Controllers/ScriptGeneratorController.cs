using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models.Dummy;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBeeWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScriptGeneratorController : ControllerBase
    {
        [HttpGet("{modelType}/{scriptType}")]
        public IActionResult Get(string modelType, string scriptType)
        {
            FileContentProvider fileContentProvider = FileContentProvider.Instance;

            switch (modelType)
            {
                case "dummy":
                {
                    switch (scriptType)
                    {
                        case "python":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new PythonStrategyGenerator(fileContentProvider)).Generate(
                                    typeof(DummyModel));

                            var zipStream = CreateFileZipStream("script.py", generatedTemplate);

                            return File(zipStream, "application/octet-stream", "DummyPythonSampleCode.zip");
                        }
                        case "javascript":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new JavascriptStrategyGenerator(fileContentProvider))
                                    .Generate(
                                        typeof(DummyModel));

                            var zipStream = CreateFileZipStream("script.js", generatedTemplate);

                            return File(zipStream, "application/octet-stream", "DummyJavascriptSampleCode.zip");
                        }
                        case "csharp":
                        {
                            var generatedTemplate =
                                new ScriptSampleGenerator(new CSharpStrategyGenerator(fileContentProvider)).Generate(
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