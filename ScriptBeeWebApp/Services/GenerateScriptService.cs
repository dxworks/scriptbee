using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class GenerateScriptService : IGenerateScriptService
{
    private readonly IScriptGeneratorStrategyHolder _scriptGeneratorStrategyHolder;
    private readonly ILoadersHolder _loadersHolder;


    public GenerateScriptService(IScriptGeneratorStrategyHolder scriptGeneratorStrategyHolder,
        ILoadersHolder loadersHolder)
    {
        _scriptGeneratorStrategyHolder = scriptGeneratorStrategyHolder;
        _loadersHolder = loadersHolder;
    }

    public IEnumerable<string> GetSupportedLanguages()
    {
        return _scriptGeneratorStrategyHolder.GetAllStrategies().Select(x => x.Language);
    }

    public IScriptGeneratorStrategy? GetGenerationStrategy(string scriptType)
    {
        return _scriptGeneratorStrategyHolder.GetStrategy(scriptType);
    }

    public async Task<Stream> GenerateClassesZip(List<object> classes, IScriptGeneratorStrategy scriptGeneratorStrategy,
        CancellationToken cancellationToken = default)
    {
        var sampleCode =
            await new SampleCodeGenerator(scriptGeneratorStrategy, _loadersHolder).GetSampleCode(classes,
                cancellationToken);

        return CreateFileZipStream(sampleCode, scriptGeneratorStrategy.Extension);
    }

    private static Stream CreateFileZipStream(IEnumerable<SampleCodeFile> sampleCode, string extension)
    {
        var zipStream = new MemoryStream();
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var sampleCodeFile in sampleCode)
            {
                var zipArchiveEntry = zip.CreateEntry(sampleCodeFile.Name + extension);

                using var writer = new StreamWriter(zipArchiveEntry.Open());
                writer.Write(sampleCodeFile.Content);
            }
        }

        zipStream.Position = 0;
        return zipStream;
    }
}
