using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Errors;

public record ProjectDoesNotExistsError(ProjectId Id);
