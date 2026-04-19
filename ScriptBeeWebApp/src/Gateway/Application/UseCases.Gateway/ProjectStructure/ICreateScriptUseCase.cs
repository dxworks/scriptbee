using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public interface ICreateScriptUseCase
{
    Task<
        OneOf<
            Script,
            ProjectDoesNotExistsError,
            ScriptLanguageDoesNotExistsError,
            ScriptPathAlreadyExistsError
        >
    > Create(CreateScriptCommand command, CancellationToken cancellationToken);
}
