using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Tests.Common;

public static class ScriptFixture
{
    public static Script BasicScript(ProjectId projectId, ScriptId id) =>
        new(
            id,
            projectId,
            new ProjectStructureFile("path.lang"),
            new ScriptLanguage("language", ".lang"),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ]
        );
}
