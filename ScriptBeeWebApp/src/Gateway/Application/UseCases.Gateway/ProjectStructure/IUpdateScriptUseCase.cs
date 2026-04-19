using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IUpdateScriptUseCase
{
    Task<OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError>> Update(
        UpdateScriptCommand command,
        CancellationToken cancellationToken
    );

    Task<OneOf<Success, ProjectDoesNotExistsError, ScriptDoesNotExistsError>> UpdateContent(
        UpdateScriptContentCommand command,
        CancellationToken cancellationToken
    );
}
