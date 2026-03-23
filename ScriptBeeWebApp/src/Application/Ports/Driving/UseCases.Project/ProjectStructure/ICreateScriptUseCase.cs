using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface ICreateScriptUseCase
{
    Task<
        OneOf<
            Script,
            ProjectDoesNotExistsError,
            NoInstanceAllocatedForProjectError,
            ScriptLanguageDoesNotExistsError,
            ScriptPathAlreadyExistsError
        >
    > Create(CreateScriptCommand command, CancellationToken cancellationToken = default);
}
