using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public class CreateScriptService : ICreateScriptUseCase
{
    public Task<
        OneOf<
            Script,
            ProjectDoesNotExistsError,
            ScriptLanguageDoesNotExistsError,
            ScriptPathAlreadyExistsError
        >
    > Create(CreateScriptCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
