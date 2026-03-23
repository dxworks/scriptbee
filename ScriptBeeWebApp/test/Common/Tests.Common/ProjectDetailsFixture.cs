using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Tests.Common;

public static class ProjectDetailsFixture
{
    public static ProjectDetails BasicProjectDetails(ProjectId projectId) =>
        new(
            projectId,
            "project",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            []
        );

    public static ProjectDetails BasicProjectDetails(
        ProjectId projectId,
        string name,
        DateTimeOffset creationDate
    ) => BasicProjectDetails(projectId, creationDate) with { Name = name };

    public static ProjectDetails BasicProjectDetails(
        ProjectId projectId,
        DateTimeOffset creationDate
    ) => BasicProjectDetails(projectId) with { CreationDate = creationDate };

    public static ProjectDetails ProjectDetailsWithSavedFiles(
        ProjectId projectId,
        Dictionary<string, List<FileData>> savedFiles
    ) => BasicProjectDetails(projectId) with { SavedFiles = savedFiles };
}
