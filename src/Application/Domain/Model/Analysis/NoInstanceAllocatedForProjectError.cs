using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Analysis;

public record NoInstanceAllocatedForProjectError(ProjectId ProjectId);
