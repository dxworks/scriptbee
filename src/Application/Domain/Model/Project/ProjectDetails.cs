using ScriptBee.Domain.Model.File;

namespace ScriptBee.Domain.Model.Project;

public record ProjectDetails(
    ProjectId Id,
    string Name,
    DateTimeOffset CreationDate,
    IDictionary<string, List<FileData>> SavedFiles
);
