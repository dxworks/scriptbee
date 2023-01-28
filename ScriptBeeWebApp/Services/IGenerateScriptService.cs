using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBeeWebApp.Services;

public interface IGenerateScriptService
{
    IEnumerable<string> GetSupportedLanguages();
    IScriptGeneratorStrategy? GetGenerationStrategy(string scriptType);

    Task<Stream> GenerateClassesZip(IEnumerable<object> classes, IScriptGeneratorStrategy scriptGeneratorStrategy,
        CancellationToken cancellationToken = default);
}
