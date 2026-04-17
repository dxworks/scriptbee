using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Common.Plugins;

namespace ScriptBee.Service.Analysis;

public class ProjectStructureService(
    IProjectManager projectManager,
    IPluginRepository pluginRepository
) : IProjectStructureService
{
    public async Task<IEnumerable<SampleCodeFile>> GenerateModelClasses(
        List<string> languages,
        CancellationToken cancellationToken
    )
    {
        var project = projectManager.GetProject();

        var classes = project.Context.GetClasses();
        var acceptedModules = GetAcceptedModules();

        var generatorStrategies = pluginRepository.GetPlugins<IScriptGeneratorStrategy>();
        var allGeneratedFiles = new List<SampleCodeFile>();

        foreach (var generatorStrategy in generatorStrategies)
        {
            if (
                languages.Count > 0
                && !languages.Any(l =>
                    string.Equals(l, generatorStrategy.Language, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                continue;
            }

            var generatedClasses = (
                await new SampleCodeGenerator(generatorStrategy, acceptedModules).GetSampleCode(
                    classes,
                    cancellationToken
                )
            ).ToList();

            allGeneratedFiles.AddRange(
                generatedClasses.Select(f =>
                    f with
                    {
                        Name = Path.Combine(
                            generatorStrategy.Language,
                            f.Name + generatorStrategy.Extension
                        ),
                    }
                )
            );
        }

        return allGeneratedFiles;
    }

    private HashSet<string> GetAcceptedModules()
    {
        var acceptedModules = new HashSet<string>();

        foreach (var modelLoader in pluginRepository.GetPlugins<IModelLoader>())
        {
            acceptedModules.Add(modelLoader.GetType().Module.Name);
        }

        return acceptedModules;
    }
}
