using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway;

public record NewProjectTokenResult(ProjectToken Token, string UnhashedToken);
