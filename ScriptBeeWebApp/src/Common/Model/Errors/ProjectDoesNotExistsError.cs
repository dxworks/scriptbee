using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Errors;

public sealed record ProjectDoesNotExistsError(ProjectId Id);
