using ScriptBee.Domain.Model.File;

namespace ScriptBee.Domain.Model.Project;

public record ProjectDetails(
    ProjectId Id,
    string Name,
    DateTimeOffset CreationDate,
    List<FileData> SavedFiles,
    IDictionary<string, List<FileData>> LoadedFiles,
    IEnumerable<string> Linkers,
    IEnumerable<PluginInstallationConfig> InstalledPlugins
);
