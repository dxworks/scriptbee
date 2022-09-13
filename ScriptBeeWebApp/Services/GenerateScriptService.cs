﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Scripts.ScriptSampleGenerators;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class GenerateScriptService : IGenerateScriptService
{
    private readonly ILoadersService _loadersService;
    private readonly IPluginRepository _pluginRepository;

    public GenerateScriptService(ILoadersService loadersService, IPluginRepository pluginRepository)
    {
        _loadersService = loadersService;
        _pluginRepository = pluginRepository;
    }

    public IEnumerable<string> GetSupportedLanguages()
    {
        return _pluginRepository.GetLoadedPluginManifests<ScriptGeneratorPluginManifest>()
            .Select(manifest => manifest.Spec.Language);
    }

    public IScriptGeneratorStrategy? GetGenerationStrategy(string scriptType)
    {
        return _pluginRepository.GetPlugin<IScriptGeneratorStrategy>(strategy => strategy.Language == scriptType);
    }

    public async Task<Stream> GenerateClassesZip(IEnumerable<object> classes,
        IScriptGeneratorStrategy scriptGeneratorStrategy,
        CancellationToken cancellationToken = default)
    {
        var acceptedModules = _loadersService.GetAcceptedModules();

        var sampleCode =
            await new SampleCodeGenerator(scriptGeneratorStrategy, acceptedModules).GetSampleCode(classes,
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
