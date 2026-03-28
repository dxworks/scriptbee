using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Errors;

public sealed record ProjectIdAlreadyInUseError(ProjectId Id);
