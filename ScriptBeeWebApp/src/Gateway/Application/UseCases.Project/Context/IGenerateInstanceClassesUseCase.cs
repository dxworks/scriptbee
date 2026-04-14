using OneOf;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

using GenerateClassesResult = OneOf<Stream, InstanceDoesNotExistsError>;

public interface IGenerateInstanceClassesUseCase
{
    Task<GenerateClassesResult> Generate(
        GenerateClassesCommand command,
        CancellationToken cancellationToken
    );
}
