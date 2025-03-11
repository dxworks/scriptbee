using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface ICreateScriptUseCase
{
    Task<
        OneOf<
            Script,
            ProjectDoesNotExistsError,
            ScriptLanguageDoesNotExistsError,
            ScriptPathAlreadyExistsError
        >
    > Create(CreateScriptCommand command, CancellationToken cancellationToken = default);
}
