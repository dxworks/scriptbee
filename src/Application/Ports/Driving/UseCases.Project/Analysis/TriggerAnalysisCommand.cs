using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

// TODO FIXIT: add scripts
public record TriggerAnalysisCommand(
    ProjectId ProjectId,
    AnalysisInstanceImage Image,
    List<string> Loaders,
    List<string> Linkers
);
