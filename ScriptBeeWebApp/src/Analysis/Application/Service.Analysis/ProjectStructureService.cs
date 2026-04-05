using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Artifacts;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Service.Analysis;

public class ProjectStructureService(
    IProjectManager projectManager,
    IPluginRepository pluginRepository,
    IDeleteFileOrFolder deleteFileOrFolder,
    ICreateFile createFile
) : IProjectStructureService
{
    public async Task GenerateModelClasses(CancellationToken cancellationToken = default)
    {
        var project = projectManager.GetProject();

        var classes = project.Context.GetClasses();
        var acceptedModules = GetAcceptedModules();

        var generatorStrategies = pluginRepository.GetPlugins<IScriptGeneratorStrategy>();
        foreach (var generatorStrategy in generatorStrategies)
        {
            var generatedClasses = await new SampleCodeGenerator(
                generatorStrategy,
                acceptedModules
            ).GetSampleCode(classes, cancellationToken);

            WriteSampleCodeFiles(
                generatedClasses,
                ProjectId.FromValue(project.Id),
                generatorStrategy.Language,
                generatorStrategy.Extension,
                cancellationToken
            );
        }
    }

    private void WriteSampleCodeFiles(
        IEnumerable<SampleCodeFile> sampleCodeFiles,
        ProjectId projectId,
        string folderName,
        string extension,
        CancellationToken cancellationToken
    )
    {
        var deleteFolderPath = Path.Combine(ConfigFolders.GeneratedFolder, folderName);
        deleteFileOrFolder.Delete(projectId, deleteFolderPath);

        foreach (var sampleCodeFile in sampleCodeFiles)
        {
            var filePath = Path.Combine(
                ConfigFolders.GeneratedFolder,
                folderName,
                sampleCodeFile.Name + extension
            );
            createFile.Create(projectId, filePath, sampleCodeFile.Content, cancellationToken);
        }
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
