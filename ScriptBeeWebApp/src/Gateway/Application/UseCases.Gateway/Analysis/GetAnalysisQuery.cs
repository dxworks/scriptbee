using ScriptBee.Application.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Analysis;

public sealed record GetAnalysisQuery(ProjectId ProjectId, IReadOnlyList<AnalysisSort> Sort);
